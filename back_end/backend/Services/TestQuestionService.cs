
using backend.DataAccess.DAO;
using backend.Dtos.Questions;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TestQuestionService
    {
        private readonly ITestQuestionRepository _testQuestionRepository;
        private readonly IQuestionRepository _questionRepository;

        public TestQuestionService(ITestQuestionRepository testQuestionRepository, IQuestionRepository questionRepository)
        {
            _testQuestionRepository = testQuestionRepository;
            _questionRepository = questionRepository;
        }

        public async Task AddQuestionsToTestAsync(AddQuestionsToTestDto dto)
        {
            if (dto == null || dto.QuestionIds == null || !dto.QuestionIds.Any())
            {
                throw new ArgumentException("Danh sách câu hỏi không hợp lệ.");
            }

            var testQuestions = dto.QuestionIds.Select(qId => new TestQuestion
            {
                TestId = dto.TestId,
                QuestionId = qId
            }).ToList();

            await _questionRepository.MarkQuestionsAsUsed(dto.QuestionIds);
            await _testQuestionRepository.AddTestQuestions(testQuestions);
        }

        public async Task<List<TestQuestion>> GetTestQuestionsAsync(int testId)
        {
            var questions = await _testQuestionRepository.GetQuestionsByTestIdAsync(testId);

            if (questions == null || !questions.Any())
            {
                throw new KeyNotFoundException("Chưa có câu hỏi nào được thêm vào bài kiểm tra.");
            }

            return questions;
        }

        public async Task UpdateTestQuestionsAsync(AddQuestionsToTestDto dto)
        {
            if (dto == null || dto.TestId <= 0 || dto.QuestionIds == null)
            {
                throw new ArgumentException("Dữ liệu đầu vào không hợp lệ.");
            }

            var existingQuestionIds = await _testQuestionRepository.GetQuestionIdsByTestIdAsync(dto.TestId);
            var questionsToAdd = dto.QuestionIds.Except(existingQuestionIds).ToList();
            var questionsToRemove = existingQuestionIds.Except(dto.QuestionIds).ToList();

            if (questionsToAdd.Any())
            {
                var testQuestions = questionsToAdd.Select(qId => new TestQuestion
                {
                    TestId = dto.TestId,
                    QuestionId = qId
                }).ToList();
                await _questionRepository.MarkQuestionsAsUsed(questionsToAdd);
                await _testQuestionRepository.AddTestQuestions(testQuestions);
            }

            if (questionsToRemove.Any())
            {
                await _testQuestionRepository.RemoveQuestionsFromTestAsync(dto.TestId, questionsToRemove);
            }
        }

    }
}
