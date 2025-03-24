namespace ProjectPRNLamnthe180410.Models.ViewModel
{
    public class SearchViewModel
    {
        public List<LightNovel> LightNovels { get; set; }
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }
        public bool Descending { get; set; }
        public List<int> SelectedGenreIds { get; set; }
        public List<Genre> AvailableGenres { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
