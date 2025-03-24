using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Models.ViewModel;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILightNovelService _lightNovelService;
        private readonly IHubContext<LightNovelHub> _hubContext;
        private readonly IUserService _userService;
        private readonly IGenreService _genreService;


        public SearchController(ILightNovelService lightNovelService, IHubContext<LightNovelHub> hubContext, IUserService userService, IGenreService genreService)
        {
            _lightNovelService = lightNovelService;
            _hubContext = hubContext;
            _userService = userService;
            _genreService = genreService;
        }
        public async Task<IActionResult> Index(string? search, string? sortBy, bool descending = true, List<int>? genreIds = null, int page = 1, int pageSize = 9)
        {
            var allLightNovels = await _lightNovelService.GetAllLightNovelsAsync(); // Get all light novels
            var genres = await _genreService.GetAllGenresAsync(); // Get all genres

            if (genreIds == null || !genreIds.Any())
            {
                genreIds = genres.Select(g => g.Id).ToList(); // Default to all genres
            }

            // Filter novels that have at least one of the selected genres
            var filteredNovels = allLightNovels.Where(n => genreIds.Contains(n.GenreId)).ToList();

            var finalFilteredNovels = await _lightNovelService.FilterFromListAsync(
                filteredNovels, search, sortBy, descending, pageSize, page, visibleOnly: true, isBought: false, userID: null
            );

            var viewModel = new SearchViewModel
            {
                LightNovels = (List<LightNovel>)finalFilteredNovels,
                SearchQuery = search,
                SortBy = sortBy,
                Descending = descending,
                SelectedGenreIds = genreIds,
                AvailableGenres = (List<Genre>)genres,
                Page = page,
                PageSize = pageSize
            };

            return View(viewModel);
        }
    }
}
