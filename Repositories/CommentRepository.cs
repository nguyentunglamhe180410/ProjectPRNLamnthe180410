using Microsoft.EntityFrameworkCore;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;
using System;

namespace ProjectPRNLamnthe180410.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AnimeLightNovelContext _context;

        public CommentRepository(AnimeLightNovelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByLightNovelIdAsync(int lightNovelId)
        {
            return await _context.Comments
                                 .Where(c => c.LightNovelId == lightNovelId)
                                 .OrderByDescending(c => c.CreateAt)
                                 .ToListAsync();
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }

}
