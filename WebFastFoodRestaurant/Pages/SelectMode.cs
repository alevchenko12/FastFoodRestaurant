using Microsoft.AspNetCore.Mvc;

public class SelectModeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Admin()
    {
        return View("Admin");
    }

    public IActionResult Client()
    {
        return View("Client");
    }
}
