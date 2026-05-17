using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public IActionResult Index()
        {
            return View(_homeService.GetHomeViewModel());
        }

        public IActionResult About()
        {
            return View();
        }
    }
} 