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

            await _curriculumRepository.AddChaptersAsync(chapters);
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
    }
}
