using Microsoft.AspNetCore.Mvc;

namespace WebFasfoodRestaurant_2.Pages.Shared
{
    public class _Layout : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
