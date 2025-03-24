using ProjectPRNLamnthe180410.Models;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Repositories.Interface
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre?> GetByIdAsync(int id);
        Task AddAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(int id);
    }
}
