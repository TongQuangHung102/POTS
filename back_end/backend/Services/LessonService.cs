using backend.Dtos.Curriculum;
using backend.Models;
using backend.Repositories;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class LessonService
    {
        private readonly ICurriculumRepository _curriculumRepository;

        public LessonService(ICurriculumRepository curriculumRepository)
        {
            _curriculumRepository = curriculumRepository;
        }

        public async Task<List<LessonDto>> GetAllLessonByChapterIdAsync(int chapterId)
        {
            var lessons = await _curriculumRepository.GetLessonByChapterIdAsync(chapterId);
            return lessons.Select(l => new LessonDto
            {
                LessonId = l.LessonId,
                LessonName = l.LessonName,
                Order = l.Order,
                IsVisible = l.IsVisible,
            }).ToList();
        }

        public async Task<Lesson> GetLessonWithQuestionsByIdAsync(int id)
        {
            return await _curriculumRepository.GetLessonWithQuestionsByIdAsync(id);
        }

        public async Task<Lesson> GetLessonWithQuestionsAIByIdAsync(int id)
        {
            return await _curriculumRepository.GetLessonWithQuestionsAIByIdAsync(id);
        }

        public async Task AddLessonsFromStringAsync(int chapterId, string input)
        {
            var lessons = ParseLessons(input, chapterId);

            await ValidateDuplicateLessonsAsync(chapterId, lessons);

            await _curriculumRepository.AddLessonsAsync(lessons);
        }
        public async Task EditLessonAsync(int id, LessonDto lessonDto)
        {
            var existingLesson = await _curriculumRepository.GetLessonByIdAsync(id);
            
            var duplicateLesson = await _curriculumRepository.GetLessonByChapterIdAsync(existingLesson.ChapterId);
            if (duplicateLesson.Any(ch => (ch.Order == lessonDto.Order || ch.LessonName == lessonDto.LessonName) && ch.LessonId != id))
            {
                throw new InvalidOperationException("Một bài có cùng thứ tự hoặc tên đã tồn tại.");
            }

            existingLesson.Order = lessonDto.Order;
            existingLesson.LessonName = lessonDto.LessonName;
            existingLesson.IsVisible = lessonDto.IsVisible;

            await _curriculumRepository.UpdateLessonAsync(existingLesson);
        }

        private static List<Lesson> ParseLessons(string input, int chapterId)
        {
            var lessons = new List<Lesson>();
            var regex = new Regex(@"Bài\s(\d+):?\s(.+?)(?=\s*Bài\s|\s*$)", RegexOptions.Singleline);
            var matches = regex.Matches(input);

            if (matches.Count == 0)
            {
                throw new FormatException("Định dạng bài học không hợp lệ. Vui lòng nhập theo mẫu: 'Bài <số>: <tên bài>'");
            }

            var lessonOrders = new HashSet<int>();

            foreach (Match match in matches)
            {
                if (!int.TryParse(match.Groups[1].Value, out int lessonOrder))
                {
                    throw new FormatException($"Số thứ tự bài học không hợp lệ: '{match.Groups[1].Value}'");
                }

                string lessonName = match.Groups[2].Value.Trim();
                if (string.IsNullOrWhiteSpace(lessonName))
                {
                    throw new FormatException($"Tên bài học không được để trống: 'Bài {lessonOrder}'");
                }

                if (lessonOrders.Contains(lessonOrder))
                {
                    throw new ArgumentException($"Bài học bị trùng lặp: Bài {lessonOrder}");
                }

                lessons.Add(new Lesson
                {
                    Order = lessonOrder,
                    LessonName = lessonName,
                    IsVisible = true,
                    ChapterId = chapterId
                });

                lessonOrders.Add(lessonOrder);
            }

            return lessons;
        }

        private async Task ValidateDuplicateLessonsAsync(int chapterId, List<Lesson> lessons)
        {
            var existingLessons = await _curriculumRepository.GetLessonByChapterIdAsync(chapterId);

            var duplicateLessons = lessons.Where(l =>
                existingLessons.Any(el => el.Order == l.Order || el.LessonName == l.LessonName)
            ).ToList();

            if (duplicateLessons.Any())
            {
                var duplicatesInfo = string.Join(", ", duplicateLessons.Select(l => $"Bài: {l.Order}: {l.LessonName}"));
                throw new InvalidOperationException($"Bài đã tồn tại: {duplicatesInfo}");
            }
        }


    }
}
