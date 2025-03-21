using backend.Dtos.Subject;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class SubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<List<Subject>> GetAllSubjects()
        {
            return await _subjectRepository.GetAllSubjectAsync();
        }

        public async Task AddNewSubjectAsync(SubjectDto dto)
        {
            var existingSubject = await _subjectRepository.GetSubjectByName(dto.SubjectName);

            if (existingSubject != null)
            {
                throw new ArgumentException("Môn học đã tồn tại.");
            }
            var subject = new Subject{
                SubjectName = dto.SubjectName,
                IsVisible = true
            };
            await _subjectRepository.AddSubjectAsync(subject);
        }

        public async Task EditSubjectAsync(SubjectEditDto dto)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new ArgumentException($"Không tìm thấy môn học với ID {dto.SubjectId}");
            } 

            subject.IsVisible = dto.IsVisible;

            await _subjectRepository.UpdateSubjectAsync(subject);
        }
    }
}
