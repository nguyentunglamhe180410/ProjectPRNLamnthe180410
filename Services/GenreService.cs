using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories;
using ProjectPRNLamnthe180410.Repositories.Interface;
using ProjectPRNLamnthe180410.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }

        public async Task<Genre?> GetGenreByIdAsync(int id)
        {
            return await _genreRepository.GetByIdAsync(id);
        }

        public async Task AddGenreAsync(Genre genre)
        {
            await _genreRepository.AddAsync(genre);
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            await _genreRepository.UpdateAsync(genre);
        }

        public async Task DeleteGenreAsync(int id)
        {
            await _genreRepository.DeleteAsync(id);
        }
    }
}
