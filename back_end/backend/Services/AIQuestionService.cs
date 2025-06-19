using backend.Dtos.AIQuestions;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;


namespace backend.Services
{
    public class AIQuestionService
    {
        private readonly HttpClient _httpClient;
        private readonly IAIQuestionRepository _repository;
        private readonly ICurriculumRepository _curriculumRepository;

        public AIQuestionService(HttpClient httpClient, IAIQuestionRepository repository, ICurriculumRepository curriculumRepository)
        {
            _httpClient = httpClient;
            _repository = repository;
            _curriculumRepository = curriculumRepository;
        }

        public async Task<List<int>> GenerateAndSaveAIQuestions(AIQuestionRequestCreate request)
        {
            var generatedQuestionIds = new List<int>();

            try
            {
                var lesson = await _curriculumRepository.GetAllInfoLessonByIdAsync(request.LessonId);

                var jsonRequest = JsonConvert.SerializeObject(new AIQuestionRequestDto
                {
                    Question = "Bài " + lesson.Order + ": " + lesson.LessonName,
                    NumQuestions = request.Num,
                    CollectionName = lesson.Chapter.SubjectGrade.CollectionName + lesson.Chapter.Semester.ToString(),
                });

                Console.WriteLine("Dữ liệu đi về là: " + JsonConvert.SerializeObject(jsonRequest, Formatting.Indented));

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5000/generate-mcq", content);
                //var response = await _httpClient.PostAsync("http://ai:5000/generate-mcq", content);

                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return generatedQuestionIds;

                var questionsResponse = JsonConvert.DeserializeObject<AIQuestionResponseDto>(responseBody);
                Console.WriteLine("Dữ liệu gửi về là: " + JsonConvert.SerializeObject(questionsResponse, Formatting.Indented));
                if (questionsResponse == null || questionsResponse.Questions.Count == 0)
                    return generatedQuestionIds;

                foreach (var questionDto in questionsResponse.Questions)
                {
                    var aiQuestion = new Models.AIQuestion
                    {
                        QuestionText = questionDto.QuestionText,
                        LevelId = questionDto.LevelId,
                        CorrectAnswer = questionDto.CorrectAnswer,
                        CreateAt = DateTime.UtcNow.AddHours(7),
                        Status = "Pending",
                        LessonId = request.LessonId
                    };

                    await _repository.SaveAIQuestionAsync(aiQuestion);
                    await Task.Delay(200); 
                    int aiQuestionId = aiQuestion.QuestionId;
                    generatedQuestionIds.Add(aiQuestionId);

                    foreach (var answerDto in questionDto.AnswerQuestions)
                    {
                        var aiAnswer = new AnswerQuestion
                        {
                            AnswerText = answerDto.AnswerText,
                            Number = answerDto.Number,
                            QuestionAiId = aiQuestionId
                        };
                        await _repository.SaveAnswerAsync(aiAnswer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
            }

            return generatedQuestionIds;
        }



        public async Task<(List<Models.AIQuestion>, int)> GetAIQuestionsByFilters(
            int lessonId, int? levelId, string? status, DateTime? createdAt, int pageNumber, int pageSize)
        {
            return await _repository.GetAIQuestionsByFilters(lessonId, levelId, status, createdAt, pageNumber, pageSize);
        }

        public async Task<bool> UpdateLessonIdAsync(int lessonId, List<int> aiQuestionIds)
        {
            return await _repository.UpdateLessonIdAsync(lessonId, aiQuestionIds);
        }

        public async Task<AiQuestionsDto?> GetAIQuestionByIdAsync(int questionId)
        {
            var question = await _repository.GetAIQuestionByIdAsync(questionId);
            if (question == null) return null;

            return new AiQuestionsDto
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                LevelId = question.LevelId,
                CorrectAnswer = question.CorrectAnswer,
                Status = question.Status,
                AnswerQuestions = question.AnswerQuestions.Select(a => new AIAnswer
                {
                    AnswerText = a.AnswerText,
                    Number = a.Number
                }).ToList()
            };
        }

        public async Task<bool> UpdateAIQuestionAsync(int questionId, AiQuestionsDto updatedQuestion)
        {
            var existingQuestion = await _repository.GetAIQuestionByIdAsync(questionId);
            if (existingQuestion == null)
            {
                return false; 
            }


            existingQuestion.QuestionText = updatedQuestion.QuestionText;
            existingQuestion.LevelId = updatedQuestion.LevelId;
            existingQuestion.CorrectAnswer = updatedQuestion.CorrectAnswer;

         
            await _repository.DeleteAnswersByQuestionId(questionId);
            foreach (var answerDto in updatedQuestion.AnswerQuestions)
            {
                var newAnswer = new AnswerQuestion
                {
                    AnswerText = answerDto.AnswerText,
                    Number = answerDto.Number,
                    QuestionAiId = questionId
                };
                await _repository.SaveAnswerAsync(newAnswer);
            }

            return await _repository.UpdateAIQuestionAsync(existingQuestion);
        }
        public async Task<bool> ApproveAIQuestionAsync(int questionId)
        {
            var aiQuestion = await _repository.GetAIQuestionByIdAsync(questionId);
            if (aiQuestion == null || aiQuestion.Status != "Pending")
            {
                return false;
            }


            aiQuestion.Status = "Approved";
            await _repository.UpdateAIQuestionAsync(aiQuestion);


            var newQuestion = new Question
            {
                QuestionText = aiQuestion.QuestionText,
                CreateAt = DateTime.UtcNow.AddHours(7),
                LevelId = aiQuestion.LevelId,
                CorrectAnswer = aiQuestion.CorrectAnswer,
                IsVisible = true,
                CreateByAI = true,
                LessonId = aiQuestion.LessonId
            };

            int newQuestionId = await _repository.SaveQuestionAsync(newQuestion);

            foreach (var aiAnswer in aiQuestion.AnswerQuestions)
            {
                aiAnswer.QuestionId = newQuestionId;
                await _repository.UpdateAnswerAsync(aiAnswer);
            }

            return true;
        }


    }

}
