using backend.Dtos.Curriculum;
using backend.Models;
using backend.Repositories;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class GradeService
    {
        private readonly IGradeRepository _gradeRepository;

        public GradeService(IGradeRepository gradeRepository)
        {
            _gradeRepository = gradeRepository;
        }

        public async Task<IEnumerable<Grades>> GetAllGradesAsync()
        {
            var grades = await _gradeRepository.GetAllGradesAsync();
            return grades ?? new List<Grades>();
        }


        public async Task<Grades> GetGradeByIdAsync(int id)
        {
            var grade = await _gradeRepository.GetGradeByIdAsync(id);
            if (grade == null)
            {
                throw new KeyNotFoundException("Grade không tồn tại.");
            }
            return grade;
        }

        public async Task<IEnumerable<Grades>> GetGradeByUserIdAsync(int id)
        {
            var grades = await _gradeRepository.GetGradeByUserIdAsync(id);
            if (grades == null || grades.Count == 0)
            {
                throw new KeyNotFoundException("Chưa có lớp nào");
            }
            return grades;
        }

        public async Task UpdateGradeAsync(int gradeId, GradeDto gradeDto)
        {
            var existingGrade = await _gradeRepository.GetGradeByIdAsync(gradeId);
            if (existingGrade == null)
            {
                throw new KeyNotFoundException("Grade không tồn tại");
            }
            var existingGrades = await GetAllGradesAsync();

            if (existingGrades.Any(g => g.GradeId != gradeId &&
       string.Equals(g.GradeName, gradeDto.GradeName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Tên khối đã tồn tại");
            }

            existingGrade.GradeName = gradeDto.GradeName;
            existingGrade.Description = gradeDto.Description;
            existingGrade.IsVisible = gradeDto.IsVisible;

            if(gradeDto.IsVisible == false || gradeDto.UserId == 0)
            {
                existingGrade.UserId = null;
            }

            if (gradeDto.UserId != 0 && gradeDto.IsVisible == true)
            {
                existingGrade.UserId = gradeDto.UserId;
            }

            await _gradeRepository.UpdateGradeAsync(existingGrade);
        }

        public async Task AddGradeAsync(string grade)
        {
            var regex = new Regex(@"^khối\s\d+$", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(grade))
            {
                throw new ArgumentException("Tên khối không đúng định dạng. Ví dụ hợp lệ: 'Khối 10'");
            }

            // Chuẩn hóa: "khối 10" → "Khối 10"
            grade = char.ToUpper(grade[0]) + grade.Substring(1).ToLower();

            var existingGrades = await GetAllGradesAsync();

            if (existingGrades.Any(g => string.Equals(g.GradeName, grade, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Tên khối đã tồn tại");
            }

            var newGrade = new Grades
            {
                GradeName = grade,
                Description = grade,
                IsVisible = true
            };

            await _gradeRepository.AddGradeAsync(newGrade);
        }



        public async Task AssignContentManagersAsync(GradeAssignment assignments)
        {
            var grade = await _gradeRepository.GetGradeByIdAsync(assignments.GradeId);
            if (grade == null)
            {
                throw new KeyNotFoundException("Khối lớp không tồn tại");
            }

            grade.UserId = assignments.UserId;
            await _gradeRepository.UpdateGradeAsync(grade);
        }
    }
}
