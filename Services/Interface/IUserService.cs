using ProjectPRNLamnthe180410.Models;

namespace ProjectPRNLamnthe180410.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task RegisterUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> ValidateUserLoginAsync(string email, string password);
        Task UpdateCoinsAsync(int userId, int amount);
        Task ChangeProfilePictureAsync(int userId, string profilePictureUrl);
        Task<bool> HasUserBoughtLightNovelAsync(int userId, int lightNovelId);
        Task<int> GetUserCoinsAsync(int id);

    }

}
