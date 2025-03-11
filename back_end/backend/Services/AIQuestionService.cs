using backend.Dtos;
using backend.Models;
using backend.Repositories;
using Newtonsoft.Json;
using System.Text;


namespace backend.Services
{
    public class AIQuestionService
    {
        private readonly HttpClient _httpClient;
        private readonly IAIQuestionRepository _repository;

        public AIQuestionService(HttpClient httpClient, IAIQuestionRepository repository)
        {
            _httpClient = httpClient;
            _repository = repository;
        }

        public async Task<List<int>> GenerateAndSaveAIQuestions(AIQuestionRequestDto request)
        {
            Console.WriteLine("===== DỮ LIỆU NHẬN ĐƯỢC TỪ CONTROLLER =====");
            Console.WriteLine("Question: " + request.Question);
            Console.WriteLine("NumQuestions: " + request.NumQuestions);

            var jsonRequest = JsonConvert.SerializeObject(request);
            Console.WriteLine("===== JSON GỬI ĐI ĐẾN FLASK =====");
            Console.WriteLine(jsonRequest);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://127.0.0.1:5000/generate-mcq", content);

            Console.WriteLine("===== KẾT QUẢ NHẬN TỪ FLASK =====");
            Console.WriteLine("StatusCode: " + response.StatusCode);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Body: " + responseBody);

            if (!response.IsSuccessStatusCode)
                return new List<int>();

            var questionsResponse = JsonConvert.DeserializeObject<AIQuestionResponseDto>(responseBody);
            if (questionsResponse == null || questionsResponse.Questions.Count == 0)
                return new List<int>();

            List<int> generatedQuestionIds = new List<int>();

            foreach (var questionDto in questionsResponse.Questions)
            {
                var aiQuestion = new Models.AIQuestion
                {
                    QuestionText = questionDto.QuestionText,
                    LevelId = questionDto.LevelId,
                    CorrectAnswer = questionDto.CorrectAnswer,
                    CreateAt = DateTime.UtcNow,
                    CreateByAI = true,
                    Status = "Pending",
                    LessonId = 1
                };

                await _repository.SaveAIQuestionAsync(aiQuestion);
                await Task.Delay(100);
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


    }

}
