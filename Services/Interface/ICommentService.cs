using ProjectPRNLamnthe180410.Models;

namespace ProjectPRNLamnthe180410.Services.Interface
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetCommentsByLightNovelIdAsync(int lightNovelId);
        Task<Comment> AddCommentAsync(int lightNovelId, int userId, string content);
    }

}
