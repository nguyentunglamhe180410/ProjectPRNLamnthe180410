using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories;
using ProjectPRNLamnthe180410.Repositories.Interface;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;

        public CommentService(ICommentRepository commentRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByLightNovelIdAsync(int lightNovelId)
        {
            var result = await _commentRepository.GetCommentsByLightNovelIdAsync(lightNovelId);
            foreach (var comment in result) {
                 comment.User = await _userRepository.GetUserByIdAsync(comment.UserId);

            }
            return result; 
        }

        public async Task<Comment> AddCommentAsync(int lightNovelId, int userId, string content)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var comment = new Comment
            {
                LightNovelId = lightNovelId,
                UserId = userId,
                Content = content,
                CreateAt = DateTime.Now,
                User = user
                
            };

            return await _commentRepository.AddCommentAsync(comment);
        }
    }

}
