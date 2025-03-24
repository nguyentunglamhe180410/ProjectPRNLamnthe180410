using Microsoft.EntityFrameworkCore;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;
using System;

namespace ProjectPRNLamnthe180410.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AnimeLightNovelContext _context;

        public UserRepository(AnimeLightNovelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await SaveChangesAsync();
            }
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> HasUserBoughtLightNovelAsync(int userId, int lightNovelId)
        {
            return await _context.Boughts
                .AnyAsync(p => p.UserId == userId && p.TitleId == lightNovelId);
        }
    }

}
