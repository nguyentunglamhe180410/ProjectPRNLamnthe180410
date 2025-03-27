using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services.Interface;
using System.Threading.Tasks;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions;

namespace ProjectPRNLamnthe180410.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IHubContext<LightNovelHub> _hubContext; // Changed to AdminHub
        private const int PageSize = 5;

        public GenreController(IGenreService genreService, IHubContext<LightNovelHub> hubContext)
        {
            _genreService = genreService;
            _hubContext = hubContext;
        }

        private bool IsAdmin() => HttpContext.Session.GetInt32("UserID") == -1;

        public async Task<IActionResult> Index(string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");

            var genres = await _genreService.GetAllGenresAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                genres = genres.Where(g => g.Name.ToLower().Contains(searchQuery.ToLower())).ToList();
            }

            // Pagination with X.PagedList
            int pageNumber = page ?? 1;
            var pagedGenres = genres.ToPagedList(pageNumber, PageSize);

            ViewBag.SearchQuery = searchQuery;

            // Broadcast search and page changes
            await _hubContext.Clients.All.SendAsync("ReceiveGenrePageUpdate", searchQuery, pageNumber);

            return View(pagedGenres);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Genre genre, string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");

            await _genreService.AddGenreAsync(genre);
            await _hubContext.Clients.All.SendAsync("ReceiveGenreUpdate", $"Genre Created: {genre.Name}");
            return RedirectToAction("Index", new { searchQuery, page });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Genre genre, string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");

            await _genreService.UpdateGenreAsync(genre);
            await _hubContext.Clients.All.SendAsync("ReceiveGenreUpdate", $"Genre Updated: {genre.Name}");
            return RedirectToAction("Index", new { searchQuery, page });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string searchQuery = "", int? page = 1)
        {
            if (!IsAdmin()) return View("~/Views/Shared/NoAuthorization.cshtml");

            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null) return NotFound();

            await _genreService.DeleteGenreAsync(id);
            await _hubContext.Clients.All.SendAsync("ReceiveGenreUpdate", $"Genre Deleted: {genre.Name}");
            return RedirectToAction("Index", new { searchQuery, page });
        }
    }
}