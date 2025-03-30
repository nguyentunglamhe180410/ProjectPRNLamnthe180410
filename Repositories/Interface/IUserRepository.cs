using ProjectPRNLamnthe180410.Models;
using System.Threading.Tasks;

namespace ProjectPRNLamnthe180410.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task SaveChangesAsync();
        Task<bool> HasUserBoughtLightNovelAsync(int userId, int lightNovelId);
        Task UpdateHistoryAsync(History history);
        Task<History> GetHistoryByIdAsync(int id);
        Task AddHistoryAsync(History history);
    }
}
