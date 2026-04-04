using Eventra.Data;
using Eventra.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new HomeViewModel
            {
                FeaturedEvents = _context.Events
                    .Where(e => e.Status == "Approved")
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(6)
                    .ToList(),

                LatestReviews = _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(3)
                    .ToList()
            };

            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }
    }
} 