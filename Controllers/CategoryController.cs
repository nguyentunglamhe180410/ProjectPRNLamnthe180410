using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services;
using ProjectPRNLamnthe180410.Services.Interface;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly ILightNovelService _lightNovelService;
        private readonly IHubContext<LightNovelHub> _hubContext;
        private readonly IUserService _userService;


        public CategoryController(IGenreService genreService, ILightNovelService lightNovelService, IHubContext<LightNovelHub> hubContext, IUserService userService)
        {
            _genreService = genreService;
            _lightNovelService = lightNovelService;
            _hubContext = hubContext;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int id, int page = 1, int pageSize = 6, string? search = null, string? sortBy = "title", bool descending = false)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null)
            {
                var user = await _userService.GetUserByIdAsync((int)userId);
                ViewData["User"] = user;
            }
            var lightNovelsByGenreID = await _lightNovelService.GetLightNovelsByGenreIdAsync(id);
            var lightNovels = await _lightNovelService.FilterFromListAsync( lightNovelsByGenreID, search, sortBy, descending, pageSize, page, true, false, userId);

            ViewData["LightNovels"] = lightNovels;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["GenreId"] = id;
            var name = await _genreService.GetGenreByIdAsync(id);
            ViewData["Genre"] = name.Name;
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", lightNovels);
            return View();
        }

    }
}
