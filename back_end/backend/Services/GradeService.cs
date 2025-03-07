using backend.Dtos;
using backend.Models;
using backend.Repositories;

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
            if (grades == null || grades.Count == 0)
            {
                throw new KeyNotFoundException("Không có grade nào trong hệ thống.");
            }
            return grades;
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
                throw new KeyNotFoundException("Chưa có lớp nào.");
            }
            return grades;
        }

        public async Task UpdateGradeAsync(int gradeId, GradeDto gradeDto)
        {
            var existingGrade = await _gradeRepository.GetGradeByIdAsync(gradeId);
            if (existingGrade == null)
            {
                throw new KeyNotFoundException("Grade không tồn tại.");
            }

            existingGrade.GradeName = gradeDto.GradeName;
            existingGrade.Description = gradeDto.Description;
            existingGrade.IsVisible = gradeDto.IsVisible;

            if (gradeDto.UserId != 0)
            {
                existingGrade.UserId = gradeDto.UserId;
            }

            await _gradeRepository.UpdateGradeAsync(existingGrade);
        }

        public async Task AddGradeAsync(GradeDto gradeDto)
        {
            var newGrade = new Grades
            {
                GradeName = gradeDto.GradeName,
                Description = gradeDto.Description,
                IsVisible = gradeDto.IsVisible
            };

            await _gradeRepository.AddGradeAsync(newGrade);
        }

        public async Task AssignContentManagersAsync(GradeAssignment assignments)
        {
            var grade = await _gradeRepository.GetGradeByIdAsync(assignments.GradeId);
            if (grade == null)
            {
                throw new KeyNotFoundException("Khối lớp không tồn tại.");
            }

            grade.UserId = assignments.UserId;
            await _gradeRepository.UpdateGradeAsync(grade);
        }
    }
}
