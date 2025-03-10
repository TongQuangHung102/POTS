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

        public async Task<bool> GenerateAndSaveAIQuestions(AIQuestionRequestDto request)
        {
            // Debug: Kiểm tra dữ liệu nhận được từ Controller
            Console.WriteLine("===== DỮ LIỆU NHẬN ĐƯỢC TỪ CONTROLLER =====");
            Console.WriteLine("Question: " + request.Question);
            Console.WriteLine("NumQuestions: " + request.NumQuestions);

            var jsonRequest = JsonConvert.SerializeObject(request);


            // Debug: Kiểm tra JSON đã serialize
            Console.WriteLine("===== JSON GỬI ĐI ĐẾN FLASK =====");
            Console.WriteLine(jsonRequest);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Debug: Kiểm tra Content-Type gửi đi
            Console.WriteLine("Content-Type gửi đi: " + content.Headers.ContentType);

            var response = await _httpClient.PostAsync("http://127.0.0.1:5000/generate-mcq", content);

            // Debug: Kiểm tra kết quả phản hồi từ Flask
            Console.WriteLine("===== KẾT QUẢ NHẬN TỪ FLASK =====");
            Console.WriteLine("StatusCode: " + response.StatusCode);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Body: " + responseBody);

            if (!response.IsSuccessStatusCode)
                return false;

            var questionsResponse = JsonConvert.DeserializeObject<AIQuestionResponseDto>(responseBody);


            if (questionsResponse == null || questionsResponse.Questions.Count == 0)
                return false;

            foreach (var questionDto in questionsResponse.Questions)
            {
                var aiQuestion = new AIQuestion
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

            return true;
        }


    }

}
