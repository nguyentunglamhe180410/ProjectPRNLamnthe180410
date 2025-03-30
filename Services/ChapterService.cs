using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;

        public ChapterService(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }
        public async Task<Chapter?> GetChapterAsync(int lightNovelId, int episode)
        {
            return await _chapterRepository.GetChapterAsync(lightNovelId, episode);
        }
        public async Task<List<Chapter>> GetChaptersByLightNovelIdAsync(int lightNovelId)
        {
            return await _chapterRepository.GetChaptersByLightNovelIdAsync(lightNovelId);
        }

        public async Task<Chapter> GetChapterByIdAsync(int chapterId)
        {
            return await _chapterRepository.GetChapterByIdAsync(chapterId);
        }

        public async Task AddChapterAsync(Chapter chapter)
        {
            if (chapter == null || chapter.Chapter1 <= 0 || string.IsNullOrEmpty(chapter.Description))
            {
                throw new ArgumentException("Invalid chapter data.");
            }

            await _chapterRepository.AddChapterAsync(chapter);
        }

        public async Task UpdateChapterAsync(Chapter chapter)
        {
            if (chapter == null || chapter.Id <= 0 || chapter.Chapter1 <= 0 || string.IsNullOrEmpty(chapter.Description))
            {
                throw new ArgumentException("Invalid chapter data.");
            }

            await _chapterRepository.UpdateChapterAsync(chapter);
        }

        public async Task DeleteChapterAsync(int chapterId)
        {
            if (chapterId <= 0)
            {
                throw new ArgumentException("Invalid chapter ID.");
            }

            await _chapterRepository.DeleteChapterAsync(chapterId);
        }

        public async Task EnsureChaptersForLightNovelAsync(int lightNovelId, int episodes)
        {
            if (lightNovelId <= 0 || episodes < 0)
            {
                throw new ArgumentException("Invalid light novel ID or episodes count.");
            }

            await _chapterRepository.EnsureChaptersForLightNovelAsync(lightNovelId, episodes);
        }
    }
}
