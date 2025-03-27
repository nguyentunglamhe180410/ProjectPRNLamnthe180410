using Microsoft.EntityFrameworkCore;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Repositories
{
    public class LightNovelRepository : ILightNovelRepository
    {
        private readonly AnimeLightNovelContext _context;

        public LightNovelRepository(AnimeLightNovelContext context)
        {
            _context = context;
        }

        // Get all light novels (for admin)
        public async Task<IEnumerable<LightNovel>> GetAllAsync()
        {
            return await _context.LightNovels.Include(ln => ln.Genre).ToListAsync();
        }

        // Get only visible light novels (for user)
        public async Task<IEnumerable<LightNovel>> GetVisibleAsync()
        {
            return await _context.LightNovels
                                 .Include(ln => ln.Genre)
                                 .Where(ln => ln.Status)
                                 .ToListAsync();
        }

        public async Task<LightNovel?> GetByIdAsync(int id)
        {
            return await _context.LightNovels.Include(ln => ln.Genre)
                                             .FirstOrDefaultAsync(ln => ln.Id == id);
        }

        public async Task AddAsync(LightNovel lightNovel)
        {
            await _context.LightNovels.AddAsync(lightNovel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LightNovel lightNovel)
        {
            _context.LightNovels.Update(lightNovel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var lightNovel = await _context.LightNovels.FindAsync(id);
            if (lightNovel != null)
            {
                _context.LightNovels.Remove(lightNovel);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<LightNovel>> FilterAsync(string? search, string? sortBy, bool descending, int pageSize, int page, bool visibleOnly, bool isBought, int? userID)
        {
            var query = _context.LightNovels.AsQueryable();

            if (visibleOnly)
            {
                query = query.Where(ln => ln.Status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(ln => ln.Title.Contains(search));
            }
            if (userID.HasValue)
            {
                var boughtLightNovelIds = _context.Boughts
                    .Where(b => b.UserId == userID)
                    .Select(b => b.TitleId);

                if (isBought)
                {
                    query = query.Where(n => boughtLightNovelIds.Contains(n.Id));
                }
                else
                {
                    query = query.Where(n => !boughtLightNovelIds.Contains(n.Id));
                }
            }
            switch (sortBy)
            {
                case "coins":
                    query = descending ? query.OrderByDescending(ln => ln.Cost) : query.OrderBy(ln => ln.Cost);
                    break;
                case "name":
                    query = descending ? query.OrderByDescending(ln => ln.Title) : query.OrderBy(ln => ln.Title);
                    break;
                case "buyCount":
                    query = descending
                        ? query.OrderByDescending(ln => ln.Boughts.Count)
                        : query.OrderBy(ln => ln.Boughts.Count);
                    break;
                case "score":
                    query = descending ? query.OrderByDescending(ln => ln.Score) : query.OrderBy(ln => ln.Score);
                    break;
                case "aired":
                    query = descending ? query.OrderByDescending(ln => ln.Aired) : query.OrderBy(ln => ln.Aired);
                    break;
                case "ranking":
                    query = descending ? query.OrderByDescending(ln => ln.Ranked) : query.OrderBy(ln => ln.Ranked);
                    break;
            }

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<LightNovel>> FilterFromListAsync(IEnumerable<LightNovel> lightNovelList,string? search,string? sortBy,bool descending,int pageSize,int page, bool visibleOnly,bool isBought,int? userID)
        {
            var query = lightNovelList.AsQueryable(); 
            if (visibleOnly)
            {
                query = query.Where(ln => ln.Status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(ln => ln.Title.Contains(search));
            }

            if (userID.HasValue)
            {
                var boughtLightNovelIds = _context.Boughts
                    .Where(b => b.UserId == userID)
                    .Select(b => b.TitleId)
                    .ToHashSet();

                query = isBought
                    ? query.Where(n => boughtLightNovelIds.Contains(n.Id))
                    : query.Where(n => !boughtLightNovelIds.Contains(n.Id));
            }

            switch (sortBy)
            {
                case "coins":
                    query = descending ? query.OrderByDescending(ln => ln.Cost) : query.OrderBy(ln => ln.Cost);
                    break;
                case "name":
                    query = descending ? query.OrderByDescending(ln => ln.Title) : query.OrderBy(ln => ln.Title);
                    break;
                case "buyCount":
                    query = descending
                        ? query.OrderByDescending(ln => ln.Boughts.Count)
                        : query.OrderBy(ln => ln.Boughts.Count);
                    break;
                case "score":
                    query = descending ? query.OrderByDescending(ln => ln.Score) : query.OrderBy(ln => ln.Score);
                    break;
                case "aired":
                    query = descending ? query.OrderByDescending(ln => ln.Aired) : query.OrderBy(ln => ln.Aired);
                    break;
                case "ranking":
                    query = descending ? query.OrderByDescending(ln => ln.Ranked) : query.OrderBy(ln => ln.Ranked);
                    break;
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<IEnumerable<LightNovel>> GetLightNovelsByGenreIdAsync(int genreId)
        {
            return await  _context.LightNovels
                .Where(n => n.GenreId == genreId)
                .ToListAsync();
        }
        public async Task RecordPurchaseAsync(int userId, int lnId)
        {
            // Check if already owned
            var alreadyOwned = await _context.Boughts.AnyAsync(b => b.UserId == userId && b.TitleId == lnId);
            if (alreadyOwned)
            {
                throw new InvalidOperationException("User already owns this light novel.");
            }

            var purchase = new Bought
            {
                UserId = userId,
                TitleId = lnId
            };
            await _context.Boughts.AddAsync(purchase);
            await _context.SaveChangesAsync();
        }

    }
}
