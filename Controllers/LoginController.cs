using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services.Interface;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHubContext<LightNovelHub> _hubContext;
        private readonly IConfiguration _configuration;

        public LoginController(IUserService userService, IHubContext<LightNovelHub> hubContext, IConfiguration configuration)
        {
            _userService = userService;
            _hubContext = hubContext;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Get admin credentials from appsettings.json
            var adminEmail = _configuration["AdminCredentials:Email"];
            var adminPassword = _configuration["AdminCredentials:Password"];

            // Check if it's the admin email
            if (email == adminEmail)
            {
                if (password == adminPassword)
                {
                    // Admin login successful
                    HttpContext.Session.SetInt32("UserID", -1); // Use a special ID for admin, e.g., -1
                    HttpContext.Session.SetString("UserName", "Admin");

                    // Notify all clients about admin login
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Admin", true);

                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    // Admin password wrong
                    ViewData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Regular user login
                if (await _userService.ValidateUserLoginAsync(email, password))
                {
                    var user = await _userService.GetUserByEmailAsync(email);
                    HttpContext.Session.SetInt32("UserID", user.Id);
                    HttpContext.Session.SetString("UserName", user.Username);

                    // Notify all clients about user login
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", user.Username, true);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // User not found or password wrong
                    ViewData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            // Notify all clients about logout
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "", false);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            var existingUser = await _userService.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                ViewData["Error"] = "Email is already in use.";
                return View("Index");
            }

            var newUser = new User
            {
                Username = name,
                Email = email,
                Password = password,
                RoleId = 1
            };

            await _userService.RegisterUserAsync(newUser);

            HttpContext.Session.SetInt32("UserID", newUser.Id);
            HttpContext.Session.SetString("UserName", newUser.Username);

            // Send notification when a new user registers
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "");

            return RedirectToAction("Index", "Home");
        }
    }
}
