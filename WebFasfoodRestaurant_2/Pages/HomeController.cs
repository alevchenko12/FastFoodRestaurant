using Microsoft.AspNetCore.Mvc;

namespace WebFasfoodRestaurant_2.Pages
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

