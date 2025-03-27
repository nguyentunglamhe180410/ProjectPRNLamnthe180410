using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Services.Interface;
using ProjectPRNLamnthe180410.Models;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHubContext<LightNovelHub> _hubContext;

        public ProfileController(IUserService userService, IHubContext<LightNovelHub> hubContext)
        {
            _userService = userService;
            _hubContext = hubContext;
        }


        // Profile view
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null && userId != -1)
            {
                var user = await _userService.GetUserByIdAsync((int)userId);
                if (user == null)
                {
                    return RedirectToAction("Index", "Login"); 
                }
                ViewData["User"] = user;
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string description, IFormFile profilePicture)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null && userId != -1)
            {
                var user = await _userService.GetUserByIdAsync((int)userId);
                if (user == null)
                {
                    return RedirectToAction("Index", "Login"); 
                }

                user.Description = description;

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName); 
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }
                    user.ProfilePicture = "/images/" + fileName;
                }

                await _userService.UpdateUserAsync(user);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", $"Profile Updated: {user.Username}");

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Login"); 
            }
        }

    }
}
