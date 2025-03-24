using Microsoft.AspNetCore.Mvc;

namespace ProjectPRNLamnthe180410.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private bool IsAdmin()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            return userId.HasValue && userId.Value == -1;
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                return View("Areas\\Admin\\Views\\Shared\\NoAuthorization.cshtml");
            }
            return View();
        }
    }
}
