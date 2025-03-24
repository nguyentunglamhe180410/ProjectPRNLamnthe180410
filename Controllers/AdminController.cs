using Microsoft.AspNetCore.Mvc;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Admin/Home/Index.cshtml");
        }

        public IActionResult Settings()
        {
            return View("~/Views/Admin/Home/Settings.cshtml"); 
        }
    }
}
