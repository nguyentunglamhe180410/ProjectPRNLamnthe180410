using ProjectPRNLamnthe180410.Models;

namespace ProjectPRNLamnthe180410.Repositories.Interface
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByLightNovelIdAsync(int lightNovelId);
        Task<Comment> AddCommentAsync(Comment comment);
    }
}
