using ProjectPRNLamnthe180410.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Repositories.Interface
{
    public interface ILightNovelRepository
    {
        Task<IEnumerable<LightNovel>> GetAllAsync(); 
        Task<IEnumerable<LightNovel>> GetVisibleAsync(); 
        Task<LightNovel?> GetByIdAsync(int id);
        Task AddAsync(LightNovel lightNovel);
        Task UpdateAsync(LightNovel lightNovel);
        Task DeleteAsync(int id);
        Task<IEnumerable<LightNovel>> GetLightNovelsByGenreIdAsync(int genreId);
        Task<IEnumerable<LightNovel>> FilterAsync(string? search, string? sortBy, bool descending, int pageSize, int page, bool visibleOnly,bool isBought, int? userID);
        Task<IEnumerable<LightNovel>> FilterFromListAsync(IEnumerable<LightNovel> lightNovelList, string? search, string? sortBy, bool descending, int pageSize, int page, bool visibleOnly, bool isBought, int? userID);
        Task RecordPurchaseAsync(int userId, int lnId);
    }
}
