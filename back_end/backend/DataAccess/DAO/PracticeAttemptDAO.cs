using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class PracticeAttemptDAO
    {
        private readonly MyDbContext _context;

        public PracticeAttemptDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId)
        {
            return await _context.PracticeAttempts
            .Where(a => a.UserId == userId && a.LessonId == lessonId)
            .ToListAsync();
        }

        public async Task<int> AddPracticeAttempt(PracticeAttempt practiceAttempt)
        {
            await _context.PracticeAttempts.AddAsync(practiceAttempt);
            await _context.SaveChangesAsync(); 

            return practiceAttempt.PracticeId; 
        }
        public async Task<PracticeAttempt> GetLastAttempt(int userId, int lessonId)
        {
            return await _context.PracticeAttempts
                .Where(a => a.UserId == userId && a.LessonId == lessonId)
                .OrderByDescending(a => a.PracticeId)
                .FirstOrDefaultAsync();
        }
        public async Task<double> GetTotalPracticeTimeByDateAsync(int subjectGradeId, int userId, DateTime date)
        {
            var totalMinutes = await _context.PracticeAttempts

  
                .Where(p => p.UserId == userId && p.CreateAt.Date == date.Date && p.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .Select(p => (double?)p.TimePractice)
                .SumAsync();

            return (totalMinutes ?? 0) / 60;
        }

        public async Task<double> GetTotalPracticeTimeAllStudentByDateAsync(DateTime date, int? gradeId = null, int? subjectGradeId = null)
        {
            var query = _context.PracticeAttempts
                .Where(p => p.CreateAt.Date == date.Date) 
                .AsQueryable(); 

            if (gradeId.HasValue) 
            {
                query = query.Where(p => p.User.GradeId == gradeId.Value);
            }
            if (subjectGradeId.HasValue)
            {
                query = query.Where(p => p.Lesson.Chapter.SubjectGradeId == subjectGradeId.Value);
            }

            var totalMinutes = await query.Select(p => (double?)p.TimePractice).SumAsync();

            return (totalMinutes ?? 0) / 60;
        }



        public async Task<double> GetAverageScoreByDateAsync(int userId, DateTime date, int subjectGradeId)
        {
            var scores = await _context.PracticeAttempts

 
                .Where(p => p.UserId == userId && p.CreateAt.Date == date.Date && p.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .Select(p => p.CorrectAnswers)
                .ToListAsync(); 

            if (!scores.Any())
            {
                return 0; 
            }

            return scores.Average();
        }


        public async Task<double> GetAverageTimeByDateAsync(int userId, DateTime date, int subjectGradeId)
        {
            var time = await _context.PracticeAttempts

                .Where(p => p.UserId == userId && p.CreateAt.Date == date.Date && p.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .Select(p => p.TimePractice)
                .ToListAsync();

            if (!time.Any())
            {
                return 0;
            }

            return time.Average() / 10;
        }

        public async Task<int> GetTotalNumberPracticeAsync(int userId, int subjectGradeId)
        {
            return await _context.PracticeAttempts

                .Where(p => p.UserId == userId && p.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .CountAsync();
        }

        public async Task<double> GetAveragePracticeTimeAsync(int userId, int subjectGradeId)
        {
            var averageTime = await _context.PracticeAttempts
                .Where(p => p.UserId == userId && p.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .Select(p => (double?)p.TimePractice) 
                .AverageAsync();

            return averageTime ?? 0;
        }


        public async Task<double> GetAveragePracticeScoreAsync(int userId, int subjectGradeId)
        {
            var averageScore = await _context.PracticeAttempts
                .Where(p => p.UserId == userId && p.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .Select(p => (double?)p.CorrectAnswers) 
                .AverageAsync();

            return averageScore ?? 0;
        }


        public async Task<List<(int UserId, double AverageScore, double TotalPracticeTime)>> GetStudentDataAsync(int gradeId)
        {
            var studentData = await _context.Users
                .Where(u => u.GradeId == gradeId)
                .Select(u => new
                {
                    UserId = u.UserId,
                    AverageScore = _context.PracticeAttempts
                        .Where(p => p.UserId == u.UserId)
                        .Select(p => (double)p.CorrectAnswers / 10)
                        .DefaultIfEmpty(0)
                        .Average(),
                    TotalPracticeTime = _context.PracticeAttempts
                        .Where(p => p.UserId == u.UserId)
                        .Select(p => p.TimePractice)
                        .DefaultIfEmpty(0)
                        .Sum()
                })
                .ToListAsync();

            return studentData.Select(s => (s.UserId, s.AverageScore, s.TotalPracticeTime)).ToList();
        }

        //lay lich su cac lan lam bai cua học sinh
        public async Task<(List<PracticeAttempt> Attempts, int TotalCount)> GetPracticeAttemptsByLessonAndUserAsync(
            int lessonId, int userId, int pageNumber, int pageSize)
        {
            var query = _context.PracticeAttempts
                .Where(pa => pa.LessonId == lessonId && pa.UserId == userId)
                .OrderByDescending(pa => pa.CreateAt);

            int totalCount = await query.CountAsync();

            var attempts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(pa => new PracticeAttempt
                {
                    PracticeId = pa.PracticeId,
                    CorrectAnswers = pa.CorrectAnswers,
                    TimePractice = pa.TimePractice,
                    CreateAt = pa.CreateAt,
                })
                .ToListAsync();

            return (attempts, totalCount);
        }

        //lay chi tiet lan lam bai do
        public async Task<PracticeAttempt> GetPracticeAttemptDetailAsync(int practiceId)
        {
            return await _context.PracticeAttempts
                .Where(pa => pa.PracticeId == practiceId)
                .Include(pa => pa.StudentAnswers) 
                    .ThenInclude(sa => sa.PracticeQuestion)
                        .ThenInclude(qa => qa.AnswerPracticeQuestions)
                .FirstOrDefaultAsync();
        }


    }
}
