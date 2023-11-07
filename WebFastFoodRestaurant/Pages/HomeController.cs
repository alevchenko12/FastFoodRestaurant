using Microsoft.AspNetCore.Mvc;

namespace WebFastFoodRestaurant.Pages
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    } 
}
