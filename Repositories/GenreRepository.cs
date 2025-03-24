using Microsoft.EntityFrameworkCore;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly AnimeLightNovelContext _context;

        public GenreRepository(AnimeLightNovelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres.Include(g => g.LightNovels).ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres.Include(g => g.LightNovels)
                                        .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task AddAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }
    }
}
