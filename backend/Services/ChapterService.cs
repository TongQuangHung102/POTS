using backend.Dtos;
using backend.Models;
using backend.Repositories;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class ChapterService
    {
        private readonly ICurriculumRepository _curriculumRepository;

        public ChapterService(ICurriculumRepository curriculumRepository)
        {
            _curriculumRepository = curriculumRepository;
        }
        public async Task<List<Chapter>> GetAllChaptersAsync()
        {
            var chapters = await _curriculumRepository.GetAllChapterAsync();   
          /*  return chapters.Select(chapter => new ChapterDto
            {
                ChapterId = chapter.ChapterId,
                ChapterName = chapter.ChapterName,
                IsVisible = chapter.IsVisible,
                Order = chapter.Order
            }).ToList();*/
          return chapters;
        }

        public async Task AddChaptersAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or empty");
            }

            var chapters = ParseChapters(input);

            await ValidateDuplicateChaptersAsync(chapters);

            await _curriculumRepository.AddChaptersAsync(chapters);
        }

        public async Task EditChapterAsync(int id, ChapterDto chapterDto)
        {
            var existingChapter = await _curriculumRepository.GetChapterByIdAsync(id);
            if (existingChapter == null)
            {
                throw new KeyNotFoundException();
            }

            var duplicateChapter = await _curriculumRepository.GetAllChapterAsync();
            if (duplicateChapter.Any(ch => (ch.Order == chapterDto.Order || ch.ChapterName == chapterDto.ChapterName) && ch.ChapterId != id))
            {
                throw new InvalidOperationException("A chapter with the same Order or Name already exists.");
            }

            existingChapter.Order = chapterDto.Order;
            existingChapter.ChapterName = chapterDto.ChapterName;
            existingChapter.IsVisible = chapterDto.IsVisible;

            await _curriculumRepository.UpdateChapterAsync(existingChapter);
        }

        private static List<Chapter> ParseChapters(string input)
        {
            var chapters = new List<Chapter>();
            var regex = new Regex(@"Chương\s(\d+):?\s(.*?)(?=Chương\s|$)");
            var matches = regex.Matches(input);

            var chapterNumbers = new HashSet<int>();

            foreach (Match match in matches)
            {
                int chapterNumber = int.Parse(match.Groups[1].Value);
                string title = match.Groups[2].Value.Trim();

                if (chapterNumbers.Contains(chapterNumber))
                {
                    throw new ArgumentException($"Duplicate chapter found: Chương {chapterNumber}");
                }

                chapters.Add(new Chapter
                {
                    Order = chapterNumber,
                    ChapterName = title,
                    IsVisible = true,
                    UserId = null
                });
                chapterNumbers.Add(chapterNumber);
            }

            return chapters;
        }

        private async Task ValidateDuplicateChaptersAsync(List<Chapter> chapters)
        {
            var existingChapters = await _curriculumRepository.GetAllChapterAsync();

            var duplicateChapters = chapters.Where(ch =>
                existingChapters.Any(ec => ec.Order == ch.Order || ec.ChapterName == ch.ChapterName)
            ).ToList();

            if (duplicateChapters.Any())
            {
                var duplicatesInfo = string.Join(", ", duplicateChapters.Select(c => $"Order: {c.Order}, Name: {c.ChapterName}"));
                throw new InvalidOperationException($"Duplicate chapters found: {duplicatesInfo}");
            }
        }
    }
}
