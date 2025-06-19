using backend.Dtos.StudentAnswer;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace backend_test.Services
{
    public class StudentAnswersServiceTests
    {
        private readonly Mock<IStudentAnswerRepository> _mockRepo;
        private readonly StudentAnswersService _service;

        public StudentAnswersServiceTests()
        {
            _mockRepo = new Mock<IStudentAnswerRepository>();
            _service = new StudentAnswersService(_mockRepo.Object);
        }

        [Fact]
        public async Task SaveStudentAnswersPractice_WhenSingleAnswerProvided_ShouldCallCreateAsyncOnceWithCorrectData()
        {
  
            var practiceId = 1;
            var questionId = 101;
            var selectedAnswer = 2;
            var studentAnswers = new List<StudentAnswerDto>
            {
                new StudentAnswerDto { PracticeId = practiceId, QuestionId = questionId, SelectedAnswer = selectedAnswer }
            };
            StudentAnswer capturedStudentAnswer = null;
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<StudentAnswer>()))
                     .Callback<StudentAnswer>(sa => capturedStudentAnswer = sa)
                     .ReturnsAsync((StudentAnswer studentAnswer) => studentAnswer);

      
            await _service.SaveStudentAnswersPractice(studentAnswers);

      
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<StudentAnswer>()), Times.Once);
            Assert.NotNull(capturedStudentAnswer);
            Assert.Equal(practiceId, capturedStudentAnswer.PracticeId);
            Assert.Equal(questionId, capturedStudentAnswer.QuestionId);
            Assert.Equal(selectedAnswer, capturedStudentAnswer.SelectedAnswer);
        }

        [Fact]
        public async Task SaveStudentAnswersPractice_WhenMultipleAnswersProvided_ShouldCallCreateAsyncForEachAnswer()
        {

            var studentAnswers = new List<StudentAnswerDto>
            {
                new StudentAnswerDto { PracticeId = 1, QuestionId = 101, SelectedAnswer = 3 },
                new StudentAnswerDto { PracticeId = 1, QuestionId = 102, SelectedAnswer = 1 }
            };
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<StudentAnswer>()))
                     .ReturnsAsync((StudentAnswer studentAnswer) => studentAnswer);


            await _service.SaveStudentAnswersPractice(studentAnswers);


            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<StudentAnswer>()), Times.Exactly(studentAnswers.Count));
        }

        [Fact]
        public async Task SaveStudentAnswersPractice_WhenNoAnswersProvided_ShouldNotCallCreateAsync()
        {

            var studentAnswers = new List<StudentAnswerDto>();


            await _service.SaveStudentAnswersPractice(studentAnswers);


            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<StudentAnswer>()), Times.Never);
        }

        [Fact]
        public async Task SaveStudentAnswersPractice_ShouldThrowInvalidOperationException_WhenRepositoryThrowsInvalidOperationException()
        {

            var studentAnswers = new List<StudentAnswerDto>
            {
                new StudentAnswerDto { PracticeId = 1, QuestionId = 101, SelectedAnswer = 3 }
            };
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<StudentAnswer>()))
                     .ThrowsAsync(new InvalidOperationException("Lỗi nghiệp vụ từ repository"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SaveStudentAnswersPractice(studentAnswers));
            Assert.Equal("Đã xảy ra lỗi nghiệp vụ khi lưu câu trả lời.", exception.Message);
        }
    }
}
