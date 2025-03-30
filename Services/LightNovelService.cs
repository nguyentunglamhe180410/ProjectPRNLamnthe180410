using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories;
using ProjectPRNLamnthe180410.Repositories.Interface;
using ProjectPRNLamnthe180410.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Services
{
    public class LightNovelService : ILightNovelService
    {
        private readonly ILightNovelRepository _lightNovelRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IUserRepository _userRepository;

        public LightNovelService(ILightNovelRepository lightNovelRepository, IGenreRepository genreRepository, IUserRepository userRepository)
        {
            _lightNovelRepository = lightNovelRepository;
            _genreRepository = genreRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<LightNovel>> GetAllLightNovelsAsync()
        {
            return await _lightNovelRepository.GetAllAsync();
        }

        public async Task<IEnumerable<LightNovel>> GetVisibleLightNovelsAsync()
        {
            return await _lightNovelRepository.GetVisibleAsync();
        }

        public async Task<LightNovel?> GetLightNovelByIdAsync(int id)
        {
            return await _lightNovelRepository.GetByIdAsync(id);
        }

        public async Task AddLightNovelAsync(LightNovel lightNovel)
        {
            await _lightNovelRepository.AddAsync(lightNovel);
        }

        public async Task UpdateLightNovelAsync(LightNovel lightNovel)
        {
            await _lightNovelRepository.UpdateAsync(lightNovel);
        }

        public async Task DeleteLightNovelAsync(int id)
        {
            await _lightNovelRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<LightNovel>> FilterAsync(string? search, string? sortBy, bool descending, int page, int amount, bool visibleOnly, bool isBought, int? userID)
        {
            var result = await _lightNovelRepository.FilterAsync(search, sortBy, descending, page, amount, visibleOnly,isBought,userID);
            if (result.Any()) 
            {
                foreach (var novel in result)
                {
                    novel.Genre = await _genreRepository.GetByIdAsync(novel.GenreId);
                }
            }
            return  result;
        }
        public async Task<IEnumerable<LightNovel>> FilterFromListAsync(IEnumerable<LightNovel> lightNovelList, string? search, string? sortBy, bool descending, int pageSize, int page, bool visibleOnly, bool isBought, int? userID)
        {
            var result = await _lightNovelRepository.FilterFromListAsync( lightNovelList, search, sortBy, descending, pageSize, page, visibleOnly, isBought, userID);
            if (result.Any())
            {
                foreach (var novel in result)
                {
                    novel.Genre = await _genreRepository.GetByIdAsync(novel.GenreId);
                }
            }
            return result;
        }
        public async Task<IEnumerable<LightNovel>> GetLightNovelsByGenreIdAsync(int genreId)
        {
            return await _lightNovelRepository.GetLightNovelsByGenreIdAsync(genreId);
        }
        public async Task<bool> CheckCanBuyAsync(int userId, int lnId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var ln = await GetLightNovelByIdAsync(lnId);

            if (user == null || ln == null) return false;

            int userCoins = user.Coins ?? 0;
            int lnCost = ln.Cost ?? 0;

            return userCoins >= lnCost; 
        }

        public async Task RecordPurchaseAsync(int userId, int id)
        {
            await _lightNovelRepository.RecordPurchaseAsync(userId, id);
        }
        public async Task UpdateRead(LightNovel lightNovel)
        {
            lightNovel.Read++;
            await _lightNovelRepository.UpdateAsync(lightNovel);
        }
    }
}
