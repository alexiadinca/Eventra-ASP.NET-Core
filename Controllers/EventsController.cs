using Eventra.Data;
using Eventra.Models;
using Eventra.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EventsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index(int? categoryId, string? city)
        {
            var query = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(e => e.City == city);

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedCity = city;

            return View(query.OrderBy(e => e.EventDate).ToList());
        }

        public IActionResult Details(int id)
        {
            var ev = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound();

            var similarEvents = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Where(e => e.Id != id && e.CategoryId == ev.CategoryId)
                .OrderByDescending(e => e.CreatedAt)
                .Take(3)
                .ToList();

            var organizerReviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.IsApproved &&
                            (r.OrganizerId == ev.OrganizerId ||
                             (r.EventId.HasValue &&
                              _context.Events.Any(x => x.Id == r.EventId && x.OrganizerId == ev.OrganizerId))))
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)
                .ToList();

            var vm = new EventDetailsViewModel
            {
                Event = ev,
                SimilarEvents = similarEvents,
                OrganizerReviews = organizerReviews
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            if (role != "Organizer")
                return Content("Only organizers can create events.");

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");

            return View(new Event
            {
                Currency = "RON",
                EventDate = DateTime.Today,
                Status = "Approved"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event model, IFormFile? imageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            if (role != "Organizer")
                return Content("Only organizers can create events.");

            model.OrganizerId = userId.Value;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;
            model.Status = "Approved";
            model.AvailableSeats = model.Capacity;

            if (model.IsFreeEntry)
            {
                model.Price = 0;
                model.Currency = "RON";
            }
            else if (string.IsNullOrWhiteSpace(model.Currency))
            {
                model.Currency = "RON";
            }

            if (!string.IsNullOrWhiteSpace(model.OrganizerDisplayName))
                model.OrganizerDisplayName = model.OrganizerDisplayName.Trim();

            ModelState.Remove("Category");
            ModelState.Remove("Organizer");
            ModelState.Remove("ApprovedByAdmin");
            ModelState.Remove("Status");
            ModelState.Remove("ImagePath");
            ModelState.Remove("OrganizerId");
            ModelState.Remove("AvailableSeats");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("ApprovedAt");
            ModelState.Remove("ApprovedByAdminId");
            ModelState.Remove("EventViews");
            ModelState.Remove("EventTagMappings");
            ModelState.Remove("EventRegistrations");
            ModelState.Remove("WaitingListEntries");
            ModelState.Remove("Favorites");
            ModelState.Remove("Reviews");

            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImagePath", "Please upload a valid image (.jpg, .jpeg, .png, .webp).");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return View(model);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "events");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    model.ImagePath = "/uploads/events/" + uniqueFileName;
                }

                _context.Events.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Event successfully created!";
                return RedirectToAction("Details", new { id = model.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                ViewBag.ErrorMessage = ex.ToString();
                return View(model);
            }
        }
    }
}