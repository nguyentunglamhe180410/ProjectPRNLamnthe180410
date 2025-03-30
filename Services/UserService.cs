using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task RegisterUserAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _userRepository.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public async Task<bool> ValidateUserLoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public async Task UpdateCoinsAsync(int userId, int amount)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Coins = (user.Coins ?? 0) + amount;
                await _userRepository.UpdateUserAsync(user);
            }
        }

        public async Task ChangeProfilePictureAsync(int userId, string profilePictureUrl)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.ProfilePicture = profilePictureUrl;
                await _userRepository.UpdateUserAsync(user);
            }
        }
        public async Task<bool> HasUserBoughtLightNovelAsync(int userId, int lightNovelId)
        {
            return await _userRepository.HasUserBoughtLightNovelAsync(userId, lightNovelId);
        }
        public async Task<int> GetUserCoinsAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            return user?.Coins ?? 0;
        }

        public async Task UpdateHistoryAsync(History history)
        {
            await _userRepository.UpdateHistoryAsync(history);
        }

        public async Task<History> GetHistoryByIdAsync(int id)
        {
            return await _userRepository.GetHistoryByIdAsync(id);

        }

        public async Task AddHistoryAsync(History history)
        {
            await _userRepository.AddHistoryAsync(history);
        }
        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("New password cannot be null or empty.", nameof(newPassword));
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.", nameof(userId));
            }

            // Hash the new password using BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Update the user in the database
            await _userRepository.UpdateUserAsync(user);
        }
    }

}
