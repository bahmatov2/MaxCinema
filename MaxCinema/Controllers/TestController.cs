using Microsoft.AspNetCore.Mvc;

namespace MaxCinema.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return Content("✅ Test controller работи!");
        }
    }
}