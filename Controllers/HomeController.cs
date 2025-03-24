using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Models.NewFolder;
using ProjectPRNLamnthe180410.Services.Interface;
using System.Diagnostics;
using System.Text.Json;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILightNovelService _lightNovelService;
        private readonly IHubContext<LightNovelHub> _hubContext;
        private readonly IUserService _userService;
        private readonly IGenreService _genreService;

        
        public HomeController(ILightNovelService lightNovelService, IHubContext<LightNovelHub> hubContext, IUserService userService, IGenreService genreService)
        {
            _lightNovelService = lightNovelService;
            _hubContext = hubContext;
            _userService = userService;
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {

            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null && userId !=-1) {
                var user = await _userService.GetUserByIdAsync((int)userId);
                ViewData["User"] = user;
            }
            var airedLightNovels = await _lightNovelService.FilterAsync(null, "aired", false, 6, 1, true,false, userId);
            var topScoredLightNovels = await _lightNovelService.FilterAsync(null, "score", true, 3, 1, true,false, userId);
            var rankedLightNovels = await _lightNovelService.FilterAsync(null, "ranked", true, 6, 1, true, false, userId);
            var mostBoughtLightNovels = await _lightNovelService.FilterAsync(null, "buyCount", true, 6, 1, true, false, userId);

            var categories = await _genreService.GetAllGenresAsync();
            var genreDTOs = categories.Select(g => new GenreDTO { Id = g.Id, Name = g.Name }).ToList();
            var categoriesJson = JsonSerializer.Serialize(genreDTOs);
            HttpContext.Session.SetString("Categories", categoriesJson);


            ViewData["AiredLightNovels"] = airedLightNovels;
            ViewData["TopScoredLightNovels"] = topScoredLightNovels;
            ViewData["RankedLightNovels"] = rankedLightNovels;
            ViewData["MostBoughtLightNovels"] = mostBoughtLightNovels;
            
            return View();
        }
    }
}
