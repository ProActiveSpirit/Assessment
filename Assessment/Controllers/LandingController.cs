using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers
{
    public class LandingController : Controller
    {
        // Action to render the Index view
        public IActionResult Index()
        {
            return View();
        }
    }
}