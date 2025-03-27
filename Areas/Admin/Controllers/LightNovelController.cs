using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services.Interface;
using X.PagedList.Extensions;

namespace ProjectPRNLamnthe180410.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LightNovelController : Controller
    {
        private readonly ILightNovelService _lightNovelService;
        private readonly IGenreService _genreService;
        private readonly IHubContext<LightNovelHub> _hubContext;
        private const int PageSize = 5; // Items per page

        public LightNovelController(ILightNovelService lightNovelService, IGenreService genreService, IHubContext<LightNovelHub> hubContext)
        {
            _lightNovelService = lightNovelService;
            _genreService = genreService;
            _hubContext = hubContext;
        }

        private bool IsAdmin() => HttpContext.Session.GetInt32("UserID") == -1;

        public async Task<IActionResult> Index(string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");

            var lightNovels = await _lightNovelService.GetAllLightNovelsAsync();
            ViewBag.Genres = await _genreService.GetAllGenresAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                lightNovels = lightNovels.Where(ln => ln.Title.ToLower().Contains(searchQuery.ToLower())).ToList();
            }

            // Pagination with X.PagedList
            int pageNumber = page ?? 1;
            var pagedLightNovels = lightNovels.ToPagedList(pageNumber, PageSize);

            ViewBag.SearchQuery = searchQuery;


            await _hubContext.Clients.All.SendAsync("ReceivePageUpdate", searchQuery, pageNumber);


            return View(pagedLightNovels);
        }

        [HttpPost]
        public async Task<IActionResult> Add(LightNovel lightNovel, string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");
            await _lightNovelService.AddLightNovelAsync(lightNovel);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", $"Light Novel Created: {lightNovel.Title}");
            return RedirectToAction("Index", new { searchQuery, page });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LightNovel lightNovel, string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");
            await _lightNovelService.UpdateLightNovelAsync(lightNovel);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", $"Light Novel Updated: {lightNovel.Title}");
            return RedirectToAction("Index", new { searchQuery, page });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");
            var lightNovel = await _lightNovelService.GetLightNovelByIdAsync(id);
            if (lightNovel != null)
            {
                await _lightNovelService.DeleteLightNovelAsync(id);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", $"Light Novel Deleted: {lightNovel.Title}");
            }
            return RedirectToAction("Index", new { searchQuery, page });
        }
    }
}
