using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IHubContext<LightNovelHub> _hubContext;



        public GenreController(IGenreService genreService, IHubContext<LightNovelHub> hubContext)
        {
            _genreService = genreService;
            _hubContext = hubContext;
        }

        private bool IsAdmin() => HttpContext.Session.GetInt32("UserID") == -1;

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return View("NoAuthorization");
            var genres = await _genreService.GetAllGenresAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", genres);

            return View(genres);
        }

        public IActionResult Create()
        {
            if (!IsAdmin()) return View("NoAuthorization");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!IsAdmin()) return View("NoAuthorization");

            await _genreService.AddGenreAsync(genre);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", genre);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return View("NoAuthorization");
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null) return NotFound();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", genre);

            return View(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Genre genre)
        {
            if (!IsAdmin()) return View("NoAuthorization");

            await _genreService.UpdateGenreAsync(genre);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", genre);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return View("NoAuthorization");
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null) return NotFound();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", genre);

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return View("NoAuthorization");
            await _genreService.DeleteGenreAsync(id);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Admin", true);

            return RedirectToAction("Index");
        }
    }
}
