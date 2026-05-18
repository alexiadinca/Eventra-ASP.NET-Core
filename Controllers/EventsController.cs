using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventra.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IWebHostEnvironment _environment;

        public EventsController(IEventService eventService, IWebHostEnvironment environment)
        {
            _eventService = eventService;
            _environment = environment;
        }

        public IActionResult Index(int? categoryId, string? city)
        {
            var search = Request.Query["search"].ToString();
            var filter = Request.Query["filter"].ToString();
            var events = _eventService.GetEvents(categoryId, city, search, filter);

            ViewBag.Categories = _eventService.GetCategories();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedCity = city;
            ViewBag.Search = search;
            ViewBag.Filter = filter;

            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("Role");
            ViewBag.UserId = userId;
            ViewBag.UserRole = userRole;
            if (userId.HasValue)
                ViewBag.FavoriteIds = _eventService.GetFavoriteEventIds(userId.Value);

            var eventIds = events.Select(e => e.Id).ToList();
            ViewBag.FavoriteCounts = _eventService.GetFavoriteCounts(eventIds);

            return View(events);
        }

        public IActionResult Details(int id)
        {
            var vm = _eventService.GetEventDetails(id);
            if (vm == null)
                return NotFound();

            if (vm.IsOrganizerOwner && vm.Event.Status != "Approved")
                TempData.Remove("SuccessMessage");

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Details), new { id }) });

            var result = _eventService.RegisterForEvent(id, userId.Value);

            if (result == "PastEvent")
            {
                TempData["ErrorMessage"] = "This event has already started or passed.";
            }
            else
            {
                TempData["SuccessMessage"] = result switch
                {
                    "OrganizerOwner" => "You can edit your own event from the details page.",
                    "AlreadyRegistered" => "You are already registered for this event.",
                    "AlreadyWaiting" => "You are already on the waiting list.",
                    "WaitingList" => "You joined the waiting list.",
                    _ => "You are counted in!"
                };
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelWaitingList(int entryId, int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Details), new { id = eventId }) });

            try
            {
                _eventService.CancelWaitingList(entryId, userId.Value);
                TempData["SuccessMessage"] = "You have been removed from the waiting list.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not remove you from the waiting list.";
            }

            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Edit), new { id }) });

            if (role != "Organizer")
                return Forbid();

            var ev = _eventService.GetEventForOrganizer(id, userId.Value);
            if (ev == null)
                return NotFound();

            ViewBag.Categories = new SelectList(_eventService.GetCategories(), "Id", "Name", ev.CategoryId);
            return View("Create", ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Event model, IFormFile? imageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Edit), new { id = model.Id }) });

            if (role != "Organizer")
                return Forbid();

            if (_eventService.GetEventForOrganizer(model.Id, userId.Value) == null)
                return NotFound();

            ModelState.Remove("Category");
            ModelState.Remove("Organizer");
            ModelState.Remove("ApprovedByAdmin");
            ModelState.Remove("Status");
            ModelState.Remove("ImagePath");
            ModelState.Remove("OrganizerId");
            ModelState.Remove("OrganizerDisplayName");
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

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_eventService.GetCategories(), "Id", "Name", model.CategoryId);
                return View("Create", model);
            }

            try
            {
                _eventService.UpdateEvent(model, imageFile);
                return RedirectToAction(nameof(Submitted), new { id = model.Id });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Categories = new SelectList(_eventService.GetCategories(), "Id", "Name", model.CategoryId);
                return View("Create", model);
            }
        }

        [HttpGet]
        public IActionResult Submitted(int id)
        {
            ViewBag.EventId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Details), new { id }) });

            if (role != "Organizer")
                return Forbid();

            try
            {
                _eventService.DeleteEvent(id, userId.Value);
                TempData["SuccessMessage"] = "Event deleted successfully.";
                return RedirectToAction("Index", "Profile");
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult CheckIn(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            if (role != "Organizer")
                return Forbid();

            var ev = _eventService.GetEventForOrganizer(id, userId.Value);
            if (ev == null)
                return NotFound();

            ViewBag.Event = ev;
            ViewBag.CheckIns = _eventService.GetCheckInsForEvent(id);
            ViewBag.ScanResult = TempData["ScanResult"];
            ViewBag.ScanAttendeeName = TempData["ScanAttendeeName"];

            if (TempData["ScanAttendeeUserId"] is int attendeeUserId)
                ViewBag.ScanAttendeeUser = _eventService.GetOrganizerUser(attendeeUserId);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn(int id, string? qrToken)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            if (role != "Organizer")
                return Forbid();

            var (result, attendeeName, attendeeUserId) = _eventService.ProcessCheckIn(id, userId.Value, qrToken?.Trim() ?? "");

            TempData["ScanResult"] = result;
            TempData["ScanAttendeeName"] = attendeeName;
            if (attendeeUserId.HasValue)
                TempData["ScanAttendeeUserId"] = attendeeUserId.Value;

            return RedirectToAction(nameof(CheckIn), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Favorite(int id, string? returnUrl)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                var signInUrl = Url.Action("SignIn", "Account", new { returnUrl = Url.Action(nameof(Details), new { id }) });
                if (isAjax) return Json(new { redirect = signInUrl });
                return Redirect(signInUrl!);
            }

            var result = _eventService.ToggleFavorite(id, userId.Value);

            if (isAjax)
            {
                var newCount = _eventService.GetFavoriteCounts(new List<int> { id }).GetValueOrDefault(id, 0);
                return Json(new { result, count = newCount });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Details), new { id });
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

            var organizer = _eventService.GetOrganizerUser(userId.Value);
            if (organizer == null || !organizer.IsApproved)
            {
                TempData["SuccessMessage"] = "Your organizer account is pending admin approval. You cannot create events yet.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = new SelectList(_eventService.GetCategories(), "Id", "Name");

            return View(new Event
            {
                Currency = "RON",
                EventDate = DateTime.Today,
                Status = "PendingApproval"
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

            ModelState.Remove("Category");
            ModelState.Remove("Organizer");
            ModelState.Remove("ApprovedByAdmin");
            ModelState.Remove("Status");
            ModelState.Remove("ImagePath");
            ModelState.Remove("OrganizerId");
            ModelState.Remove("OrganizerDisplayName");
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

            if (model.EventDate.Date + model.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("EventDate", "The event date and start time cannot be in the past.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_eventService.GetCategories(), "Id", "Name");
                return View(model);
            }

            try
            {
                var createdEvent = _eventService.CreateEvent(model, imageFile, userId.Value, role);
                return RedirectToAction(nameof(Submitted), new { id = createdEvent.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Categories = new SelectList(_eventService.GetCategories(), "Id", "Name");
                if (ex.Message.Contains("valid image", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("ImagePath", ex.Message);
                }
                else
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
                return View(model);
            }
        }
    }
}