using Microsoft.AspNetCore.Mvc;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ProjectPRNLamnthe180410.Models.ViewModel;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly IUserService _userService;

        public ChangePasswordController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // GET: ChangePassword/Index
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null && userId != -1)
            {
                var user = await _userService.GetUserByIdAsync((int)userId);
                ViewData["User"] = user;
            }

            return View(new ChangePasswordViewModel());
        }

        // POST: ChangePassword/UpdatePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            User user = null;

            if (userId != null && userId != -1)
            {
                user = await _userService.GetUserByIdAsync((int)userId);
                ViewData["User"] = user;
            }


            // Verify reCAPTCHA
            var recaptchaResponse = Request.Form["g-recaptcha-response"];
            if (string.IsNullOrEmpty(recaptchaResponse))
            {
                ModelState.AddModelError("", "Please complete the CAPTCHA.");
                return View("Index", model);
            }

            // For logged-in user (Change Password)
            if (user != null)
            {
                // Verify current password using ValidateUserLoginAsync
                if (!await _userService.ValidateUserLoginAsync(user.Email, model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View("Index", model);
                }

                // Update password
                await _userService.UpdatePasswordAsync(user.Id, model.NewPassword);
                TempData["Message"] = "Password changed successfully.";
                return RedirectToAction("Index", "Profile");
            }
            // For forgot password
            else
            {
                user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "No account found with this email.");
                    return View("Index", model);
                }

                // Update password
                await _userService.UpdatePasswordAsync(user.Id, model.NewPassword);
                TempData["Message"] = "Password reset successfully. Please log in.";
                return RedirectToAction("Index", "Login");
            }
        }
    }
}