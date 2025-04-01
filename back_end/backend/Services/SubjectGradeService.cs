using backend.Dtos.SubjectGrade;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class SubjectGradeService
    {
        private readonly ISubjectGradeRepository _subjectGradeRepository;

        public SubjectGradeService(ISubjectGradeRepository subjectGradeRepository)
        {
            _subjectGradeRepository = subjectGradeRepository;
        }

        public async Task<List<SubjectGrade>> GetAllSubjectByGradeAsync(int gradeId)
        {
            return await _subjectGradeRepository.GetAllSubjectByGradeAsync(gradeId);
        }

        public async Task<SubjectGrade> GetBySubjectGradeAsync(int gradeId, int subjectId)
        {
            var subjectGrade = await _subjectGradeRepository.GetByGradeAndSubjectAsync(gradeId, subjectId);
            if (subjectGrade == null)
            {
                throw new Exception("Chưa có môn học nào ở khối này!");
            }
            return subjectGrade;
        }


        public async Task<SubjectGrade> GetTestBySubjectGradeAsync(int gradeId, int subjectId)
        {
            return await _subjectGradeRepository.GetTestByGradeAndSubjectAsync(gradeId, subjectId);
        }

        public async Task AddNewSubjectGrade(SubjectGradeDto subjectGradeDto)
        {
            var existingRecord = await _subjectGradeRepository.GetByGradeAndSubjectAsync(
                 subjectGradeDto.GradeId, subjectGradeDto.SubjectId);

            if (existingRecord != null)
            {
                throw new Exception("Môn học đã tồn tại trong khối lớp này.");
            }

            var sg = new SubjectGrade {
                GradeId = subjectGradeDto.GradeId,
                SubjectId = subjectGradeDto.SubjectId,
            };

            await _subjectGradeRepository.AddAsync(sg);
        }

    }
}
