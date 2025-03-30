using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;
using ProjectPRNLamnthe180410.Services;
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
        private readonly IChapterService _chapterService;
        public LightNovelController(ILightNovelService lightNovelService, IGenreService genreService, IHubContext<LightNovelHub> hubContext, IChapterService chapterService)
        {
            _lightNovelService = lightNovelService;
            _genreService = genreService;
            _hubContext = hubContext;
            _chapterService = chapterService;
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
        public async Task<IActionResult> Add(LightNovel lightNovel, string searchQuery, int page)
        {
            await _lightNovelService.AddLightNovelAsync(lightNovel);
            await _chapterService.EnsureChaptersForLightNovelAsync(lightNovel.Id, (int)lightNovel.Episodes);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Light novel added");
            return RedirectToAction("Index", new { searchQuery, page });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LightNovel lightNovel, string searchQuery, int page)
        {
            var updateln = await _lightNovelService.GetLightNovelByIdAsync(lightNovel.Id);
            updateln.Title = lightNovel.Title;
            updateln.Description = lightNovel.Description;
            updateln.Episodes = lightNovel.Episodes;
            updateln.ImgUrl = lightNovel.ImgUrl;
            updateln.GenreId = lightNovel.GenreId;
            updateln.Cost = lightNovel.Cost;
            updateln.Aired = lightNovel.Aired = DateOnly.FromDateTime(DateTime.Now);
            updateln.Status = lightNovel.Status;
            await _lightNovelService.UpdateLightNovelAsync(updateln);
            await _chapterService.EnsureChaptersForLightNovelAsync(lightNovel.Id, (int)lightNovel.Episodes);
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Light novel updated");
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
        [HttpGet]
        public async Task<IActionResult> GetChapters(int lightNovelId)
        {
            try
            {
                var chapters = await _chapterService.GetChaptersByLightNovelIdAsync(lightNovelId);
                var chapterData = chapters.Select(c => new
                {
                    c.Id,
                    c.Chapter1,
                    c.Description,
                    c.LightNovelId
                }).ToList();
                return Json(chapterData);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while retrieving chapters: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddChapter(Chapter chapter)
        {
            try
            {
                if (chapter.LightNovelId == 0)
                {
                    chapter.LightNovelId = (int)HttpContext.Session.GetInt32("lnID");
                }
                await _chapterService.AddChapterAsync(chapter);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Chapter added");
                return Json(new { success = true });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while adding the chapter." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditChapter(Chapter chapter)
        {
            try
            {
                if (chapter.LightNovelId <= 0)
                {
                    return Json(new { success = false, message = "Light Novel ID is required and must be a positive number." });
                }

                await _chapterService.UpdateChapterAsync(chapter);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Chapter updated");
                return Json(new { success = true });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the chapter: " + ex.Message });
            }
        }

        [HttpPost("/Admin/LightNovel/DeleteChapter/{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid chapter ID." });
                }

                await _chapterService.DeleteChapterAsync(id);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Chapter deleted");
                return Json(new { success = true });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the chapter." });
            }
        }
    }
}
