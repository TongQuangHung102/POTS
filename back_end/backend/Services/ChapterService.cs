using backend.DataAccess.DAO;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class ChapterService
    {
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IStudentPerformanceRepository _studentPerformanceRepository;

        public ChapterService(ICurriculumRepository curriculumRepository, IStudentPerformanceRepository studentPerformanceRepository)
        {
            _curriculumRepository = curriculumRepository;
            _studentPerformanceRepository = studentPerformanceRepository;
        }
        public async Task<List<Chapter>> GetAllChaptersAsync(int gradeId)
        {
            var chapters = await _curriculumRepository.GetAllChapterAsync(gradeId);

            return chapters;
        }

        public async Task AddChaptersAsync(int gradeId,int semester, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Không được bỏ trống");
            }

            var chapters = ParseChapters(input, semester);

            await ValidateDuplicateChaptersAsync(gradeId, chapters);

            await _curriculumRepository.AddChaptersAsync(chapters);
        }

        public async Task EditChapterAsync(int id, ChapterDto chapterDto)
        {
            var existingChapter = await _curriculumRepository.GetChapterByIdAsync(id);
            if (existingChapter == null)
            {
                throw new KeyNotFoundException();
            }

            var duplicateChapter = await _curriculumRepository.GetAllChapterAsync(existingChapter.GradeId);
            if (duplicateChapter.Any(ch => (ch.Order == chapterDto.Order && ch.ChapterName == chapterDto.ChapterName) && ch.ChapterId != id))
            {
                throw new InvalidOperationException("Một chương có cùng thứ tự hoặc tên đã tồn tại.");
            }

            existingChapter.Order = chapterDto.Order;
            existingChapter.ChapterName = chapterDto.ChapterName;
            existingChapter.IsVisible = chapterDto.IsVisible;
            existingChapter.Semester = chapterDto.Semester;

            await _curriculumRepository.UpdateChapterAsync(existingChapter);
        }

        private static List<Chapter> ParseChapters(string input, int semester)
        {
            var chapters = new List<Chapter>();
            var regex = new Regex(@"Chương\s(\d+):?\s(.+?)(?=\s*Chương\s|\s*$)", RegexOptions.Singleline);
            var matches = regex.Matches(input);

            if (matches.Count == 0)
            {
                throw new FormatException("Định dạng chương không hợp lệ. Vui lòng nhập theo mẫu: 'Chương <số>: <tên chương>'");
            }

            var chapterNumbers = new HashSet<int>();

            foreach (Match match in matches)
            {
                if (!int.TryParse(match.Groups[1].Value, out int chapterNumber))
                {
                    throw new FormatException($"Số thứ tự chương không hợp lệ: '{match.Groups[1].Value}'");
                }

                string title = match.Groups[2].Value.Trim();
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new FormatException($"Tên chương không được để trống: 'Chương {chapterNumber}'");
                }

                if (chapterNumbers.Contains(chapterNumber))
                {
                    throw new ArgumentException($"Chương bị trùng lặp: Chương {chapterNumber}");
                }

                chapters.Add(new Chapter
                {
                    Order = chapterNumber,
                    ChapterName = title,
                    IsVisible = true,
                    UserId = null,
                    Semester = semester
                });
                chapterNumbers.Add(chapterNumber);
            }

            return chapters;
        }


        private async Task ValidateDuplicateChaptersAsync(int gradeId, List<Chapter> chapters)
        {
            var existingChapters = await _curriculumRepository.GetAllChapterAsync(gradeId);

            var duplicateChapters = chapters.Where(ch =>
                existingChapters.Any(ec => ec.Order == ch.Order && ec.ChapterName == ch.ChapterName)
            ).ToList();

            if (duplicateChapters.Any())
            {
                var duplicatesInfo = string.Join(", ", duplicateChapters.Select(c => $"Chương: {c.Order} : {c.ChapterName}"));
                throw new InvalidOperationException($"Chương đã tồn tại : {duplicatesInfo}");
            }
        }

        public async Task<List<StudentChapterDto>> GetFilteredChaptersAsync(int grade, int studentId)
        {
            var chapters = await _curriculumRepository.GetAllChapterAsync(grade);
            var studentPerformances = await _studentPerformanceRepository.GetStudentPerformanceAsync(studentId);

            return chapters.Where(c => c.IsVisible == true).Select(chapter => new StudentChapterDto
            {
                ChapterId = chapter.ChapterId,
                Name = chapter.ChapterName,
                Order = chapter.Order,
                Lessons = chapter.Lessons.Select(lesson => new StudentLessonDto
                {
                    LessonId = lesson.LessonId,
                    LessonName = lesson.LessonName,
                    Order = lesson.Order,
                    AverageScore = studentPerformances
                                            .Where(sp => sp.LessonId == lesson.LessonId)
                                            .Select(sp => sp.avg_Accuracy)
                                            .FirstOrDefault(),
                    AverageTime = studentPerformances
                                            .Where(sp => sp.LessonId == lesson.LessonId)
                                            .Select(sp => sp.avg_Time)
                                            .FirstOrDefault()
                }).ToList()
            }).ToList();
        }
    }
}
