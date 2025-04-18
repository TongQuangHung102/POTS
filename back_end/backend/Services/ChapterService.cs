﻿using backend.DataAccess.DAO;
using backend.Dtos.Curriculum;
using backend.Dtos.Dashboard;
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
        private readonly ISubjectGradeRepository _subjectGradeRepository;

        public ChapterService(ICurriculumRepository curriculumRepository, IStudentPerformanceRepository studentPerformanceRepository, ISubjectGradeRepository subjectGradeRepository)
        {
            _curriculumRepository = curriculumRepository;
            _studentPerformanceRepository = studentPerformanceRepository;
            _subjectGradeRepository = subjectGradeRepository;
        }

        public async Task<List<Chapter>> GetAllChaptersAsync(int gradeId)
        {
            var chapters = await _curriculumRepository.GetAllChapterAsync(gradeId);

            return chapters;
        }

        public async Task<Chapter> GetChapterByIdAsync(int chapterId)
        {
            return await _curriculumRepository.GetChapterByIdAsync(chapterId);
        }

        public async Task AddChaptersAsync(int subjectgradeId, int semester, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Không được bỏ trống");
            }

            var chapters = ParseChapters(subjectgradeId, input, semester);

            await ValidateDuplicateChaptersAsync(subjectgradeId, chapters);

            await _curriculumRepository.AddChaptersAsync(chapters);
        }

        public async Task EditChapterAsync(int id, ChapterDto chapterDto)
        {
            var existingChapter = await _curriculumRepository.GetChapterByIdAsync(id);
            if (existingChapter == null)
            {
                throw new KeyNotFoundException();
            }

            var duplicateChapter = await _curriculumRepository.GetAllChapterAsync(existingChapter.SubjectGradeId);
            if (duplicateChapter.Any(ch => (ch.Order == chapterDto.Order || ch.ChapterName == chapterDto.ChapterName) && ch.ChapterId != id))
            {
                throw new InvalidOperationException("Một chương có cùng thứ tự hoặc tên đã tồn tại.");
            }

            existingChapter.Order = chapterDto.Order;
            existingChapter.ChapterName = chapterDto.ChapterName;
            existingChapter.IsVisible = chapterDto.IsVisible;
            existingChapter.Semester = chapterDto.Semester;

            await _curriculumRepository.UpdateChapterAsync(existingChapter);
        }

        private static List<Chapter> ParseChapters(int subjectgradeId, string input, int semester)
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
                    Semester = semester,
                    SubjectGradeId = subjectgradeId
                });
                chapterNumbers.Add(chapterNumber);
            }

            return chapters;
        }


        private async Task ValidateDuplicateChaptersAsync(int subjectgradeId, List<Chapter> chapters)
        {
            var existingChapters = await _curriculumRepository.GetAllChapterAsync(subjectgradeId);

            var duplicateChapters = chapters.Where(ch =>
                existingChapters.Any(ec => ec.Order == ch.Order && ec.ChapterName == ch.ChapterName)
            ).ToList();

            if (duplicateChapters.Any())
            {
                var duplicatesInfo = string.Join(", ", duplicateChapters.Select(c => $"Chương: {c.Order} : {c.ChapterName}"));
                throw new InvalidOperationException($"Chương đã tồn tại : {duplicatesInfo}");
            }
        }

        public async Task<List<StudentChapterDto>> GetFilteredChaptersAsync(int id, int studentId)
        {
            var chapters = await _curriculumRepository.GetAllChapterAsync(id);
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
        public async Task<List<ChapterWithQuestionLevelsDto>> GetChaptersWithQuestionLevelsAsync(int gradeId, int subjectId)
        {
            var subjectGrade = await _subjectGradeRepository.GetByGradeAndSubjectAsync(gradeId, subjectId);
            if (subjectGrade == null)
            {
                return new List<ChapterWithQuestionLevelsDto>(); 
            }

            var chapters = await _curriculumRepository.GetChaptersWithQuestionsBySubjectGradeAsync(subjectGrade.Id);
            if (chapters == null || chapters.Count == 0)
            {
                return new List<ChapterWithQuestionLevelsDto>();
            }

            return chapters.Select(c => new ChapterWithQuestionLevelsDto
            {
                ChapterId = c.ChapterId,
                ChapterName = c.ChapterName,
                Order = c.Order,
                Semester = c.Semester,
                IsVisible = c.IsVisible,
                QuestionLevelCounts = c.Lessons != null
                    ? c.Lessons
                        .Where(l => l.Questions != null)
                        .SelectMany(l => l.Questions)
                        .GroupBy(q => q.LevelId)
                        .ToDictionary(g => g.Key, g => g.Count())
                    : new Dictionary<int, int>()
            }).ToList();
        }

    }
}
