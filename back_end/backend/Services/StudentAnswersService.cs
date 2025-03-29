using backend.DataAccess.DAO;
using backend.Dtos.StudentAnswer;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class StudentAnswersService
    { 
        private readonly IStudentAnswerRepository _studentAnswerRepository;

        public StudentAnswersService(IStudentAnswerRepository studentAnswerRepository)
        {
            _studentAnswerRepository = studentAnswerRepository;
        }

        public async Task SaveStudentAnswersPractice(List<StudentAnswerDto> studentAnswers)
        {
            try
            {
                foreach (var answer in studentAnswers)
                {
                    var studentAnswer = new StudentAnswer
                    {
                        PracticeId = answer.PracticeId,
                        QuestionId = answer.QuestionId, 
                        SelectedAnswer = answer.SelectedAnswer 
                    };

                    await _studentAnswerRepository.CreateAsync(studentAnswer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi lưu câu trả lời của học sinh: " + ex.Message);
            }
        }
    }
    
}
