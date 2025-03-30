﻿using ProjectPRNLamnthe180410.Models;

namespace ProjectPRNLamnthe180410.Services.Interface
{
    public interface IChapterService
    {
        Task<Chapter?> GetChapterAsync(int lightNovelId, int episode);
        Task<List<Chapter>> GetChaptersByLightNovelIdAsync(int lightNovelId);
        Task<Chapter> GetChapterByIdAsync(int chapterId);
        Task AddChapterAsync(Chapter chapter);
        Task UpdateChapterAsync(Chapter chapter);
        Task DeleteChapterAsync(int chapterId);
        Task EnsureChaptersForLightNovelAsync(int lightNovelId, int episodes);
    }
}
