using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LightNovelController : Controller
    {
        private readonly ILightNovelService _lightNovelService;
        private readonly IGenreService _genreService;
        private readonly IHubContext<LightNovelHub> _hubContext;


        public LightNovelController(ILightNovelService lightNovelService, IGenreService genreService,IHubContext<LightNovelHub> hubContext)
        {
            _lightNovelService = lightNovelService;
            _genreService = genreService;
            _hubContext = hubContext;
        }

        private bool IsAdmin() => HttpContext.Session.GetInt32("UserID") == -1;

        // READ: List all light novels
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return View("NoAuthorization");
            var lightNovels = await _lightNovelService.GetAllLightNovelsAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", lightNovels);

            return View(lightNovels);
        }

        // CREATE: Show create form
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return View("NoAuthorization");
            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Admin", true);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LightNovel lightNovel)
        {
            if (!IsAdmin()) return View("NoAuthorization");
            lightNovel.Link = $"https://localhost:7247/Details/Index/{lightNovel.Id}";
            await _lightNovelService.AddLightNovelAsync(lightNovel);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", lightNovel);
            
            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            return RedirectToAction("Index");
        }

        // UPDATE: Show edit form
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return View("NoAuthorization");
            var lightNovel = await _lightNovelService.GetLightNovelByIdAsync(id);
            if (lightNovel == null) return NotFound();
            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", lightNovel);
            return View(lightNovel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LightNovel lightNovel)
        {
            if (!IsAdmin()) return View("NoAuthorization");

            await _lightNovelService.UpdateLightNovelAsync(lightNovel);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", lightNovel);

            

            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            return RedirectToAction("Index");
        }

        // DELETE: Show delete confirmation
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return View("NoAuthorization");
            var lightNovel = await _lightNovelService.GetLightNovelByIdAsync(id);
            if (lightNovel == null) return NotFound();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", lightNovel);
            return View(lightNovel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return View("NoAuthorization");
            await _lightNovelService.DeleteLightNovelAsync(id);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Admin", true);
            return RedirectToAction("Index");
        }
    }
}
