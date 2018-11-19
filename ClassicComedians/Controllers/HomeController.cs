using Microsoft.AspNetCore.Mvc;

namespace ClassicComedians.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
