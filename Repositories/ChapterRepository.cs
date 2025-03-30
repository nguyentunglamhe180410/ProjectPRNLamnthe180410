using Microsoft.EntityFrameworkCore;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Repositories.Interface;

namespace ProjectPRNLamnthe180410.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly AnimeLightNovelContext _context;

        public ChapterRepository(AnimeLightNovelContext context)
        {
            _context = context;
        }

        public async Task<Chapter?> GetChapterAsync(int lightNovelId, int episode)
        {
            return await _context.Chapters
                .FirstOrDefaultAsync(c => c.LightNovelId == lightNovelId && c.Chapter1 == episode);
        }
        public async Task<List<Chapter>> GetChaptersByLightNovelIdAsync(int lightNovelId)
        {
            return await _context.Chapters
                .Where(c => c.LightNovelId == lightNovelId)
                .OrderBy(c => c.Chapter1)
                .ToListAsync();
        }

        public async Task<Chapter> GetChapterByIdAsync(int chapterId)
        {
            return await _context.Chapters
                .FirstOrDefaultAsync(c => c.Id == chapterId);
        }

        public async Task AddChapterAsync(Chapter chapter)
        {
            // Validate chapter number against LightNovel.Episodes
            var lightNovel = await _context.LightNovels.FindAsync(chapter.LightNovelId);
            if (lightNovel == null)
            {
                throw new KeyNotFoundException("Light novel not found.");
            }

            if (chapter.Chapter1 < 1 || chapter.Chapter1 > lightNovel.Episodes)
            {
                throw new InvalidOperationException($"Chapter number must be between 1 and {lightNovel.Episodes}.");
            }

            // Check if chapter already exists
            var existingChapter = await _context.Chapters
                .FirstOrDefaultAsync(c => c.LightNovelId == chapter.LightNovelId && c.Chapter1 == chapter.Chapter1);
            if (existingChapter != null)
            {
                throw new InvalidOperationException("Chapter number already exists for this light novel.");
            }

            // Add the chapter
            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateChapterAsync(Chapter chapter)
        {
            // Validate chapter number against LightNovel.Episodes
            var lightNovel = await _context.LightNovels.FindAsync(chapter.LightNovelId);
            if (lightNovel == null)
            {
                throw new KeyNotFoundException("Light novel not found.");
            }

            if (chapter.Chapter1 < 1 || chapter.Chapter1 > lightNovel.Episodes)
            {
                throw new InvalidOperationException($"Chapter number must be between 1 and {lightNovel.Episodes}.");
            }

            // Check if another chapter with the same number exists
            var existingChapter = await _context.Chapters
                .FirstOrDefaultAsync(c => c.LightNovelId == chapter.LightNovelId && c.Chapter1 == chapter.Chapter1 && c.Id != chapter.Id);
            if (existingChapter != null)
            {
                throw new InvalidOperationException("Another chapter with this number already exists for this light novel.");
            }

            // Update the chapter
            _context.Chapters.Update(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteChapterAsync(int chapterId)
        {
            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter == null)
            {
                throw new KeyNotFoundException("Chapter not found.");
            }

            // Delete the chapter
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task EnsureChaptersForLightNovelAsync(int lightNovelId, int episodes)
        {
            var existingChapters = await _context.Chapters
                .Where(c => c.LightNovelId == lightNovelId)
                .ToListAsync();

            // Remove chapters if Episodes decreased
            var chaptersToRemove = existingChapters
                .Where(c => c.Chapter1 > episodes)
                .ToList();
            _context.Chapters.RemoveRange(chaptersToRemove);

            // Add missing chapters if Episodes increased
            for (int i = 1; i <= episodes; i++)
            {
                if (!existingChapters.Any(c => c.Chapter1 == i))
                {
                    _context.Chapters.Add(new Chapter
                    {
                        LightNovelId = lightNovelId,
                        Chapter1 = i,
                        Description = $"Chapter {i} content placeholder."
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
