using Microsoft.AspNetCore.Mvc;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services;
using ProjectPRNLamnthe180410.Services.Interface;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class ChapterController : Controller
    {
        private readonly ILightNovelService _lightNovelService;
        private readonly IUserService _userService;
        private readonly IChapterService _chapterService;

        public ChapterController(
            ILightNovelService lightNovelService,
            IUserService userService,
            IChapterService chapterService)
        {
            _lightNovelService = lightNovelService;
            _userService = userService;
            _chapterService = chapterService;
        }

        public async Task<IActionResult> Index(int lightNovelId, int chapterNumber)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var lightNovel = await _lightNovelService.GetLightNovelByIdAsync(lightNovelId);
            if (lightNovel == null)
            {
                return NotFound("Light novel not found.");
            }

            // Check if user owns the novel
            var hasBought = await _userService.HasUserBoughtLightNovelAsync((int)userId, lightNovelId);
            if (!hasBought)
            {
                TempData["Error"] = "You must purchase this light novel to read its chapters.";
                return RedirectToAction("Index", "Details", new { id = lightNovelId });
            }

            // Fetch chapter
            var chapter = await _chapterService.GetChapterAsync(lightNovelId, chapterNumber);
            if (chapter == null || chapterNumber > lightNovel.Episodes)
            {
                return NotFound("Chapter not found.");
            }

            ViewData["LightNovel"] = lightNovel;
            return View(chapter);
        }
    }
}
