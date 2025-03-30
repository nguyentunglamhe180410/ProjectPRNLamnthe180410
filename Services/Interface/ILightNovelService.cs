using ProjectPRNLamnthe180410.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Services.Interface
{
    public interface ILightNovelService
    {
        Task<IEnumerable<LightNovel>> GetAllLightNovelsAsync();
        Task<IEnumerable<LightNovel>> GetVisibleLightNovelsAsync();
        Task<LightNovel?> GetLightNovelByIdAsync(int id);
        Task AddLightNovelAsync(LightNovel lightNovel);
        Task UpdateLightNovelAsync(LightNovel lightNovel);
        Task DeleteLightNovelAsync(int id);
        Task<IEnumerable<LightNovel>> GetLightNovelsByGenreIdAsync(int genreId);
        Task<IEnumerable<LightNovel>> FilterAsync(string? search, string? sortBy, bool descending, int page, int amount, bool visibleOnly,bool isBought, int? userID);
        Task<IEnumerable<LightNovel>> FilterFromListAsync(IEnumerable<LightNovel> lightNovelList, string? search, string? sortBy, bool descending, int pageSize, int page, bool visibleOnly, bool isBought, int? userID);
        Task<bool> CheckCanBuyAsync(int userId, int lnId);
        Task RecordPurchaseAsync(int userId, int id);
        Task UpdateRead(LightNovel lightNovel);

    }
}
