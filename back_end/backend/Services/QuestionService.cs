using backend.Dtos;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace backend.Services
{
    public class QuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly HttpClient _httpClient;

        public QuestionService(IQuestionRepository questionRepository, HttpClient httpClient, ICurriculumRepository curriculumRepository)
        {
            _questionRepository = questionRepository;
            _httpClient = httpClient;
            _curriculumRepository = curriculumRepository;
        }

        public async Task<IActionResult> GetAllQuestionsAsync(int? chapterId, int? lessonId, int? levelId,string searchTerm, bool? isVisible, int page, int pageSize)
        {
            try
            {
                var totalQuestions = await _questionRepository.GetTotalQuestionsAsync(chapterId, lessonId, levelId, searchTerm, isVisible);
                var questions = await _questionRepository.GetAllQuestionsAsync(chapterId, lessonId, levelId, searchTerm, isVisible, page, pageSize);

                var response = new
                {
                    TotalQuestions = totalQuestions,
                    Page = page,
                    PageSize = pageSize,
                    Data = questions.Select(q => new
                    {
                        q.QuestionId,
                        q.QuestionText,
                        q.CreateAt,
                        q.CorrectAnswer,
                        CorrectAnswerText = q.AnswerQuestions.FirstOrDefault(a => a.Number == q.CorrectAnswer)?.AnswerText,
                        q.IsVisible,
                        q.CreateByAI,
                        Level = new
                        {
                            q.Level.LevelName,
                            q.Level.LevelId
                        },
                        Lesson = new
                        {
                            q.Lesson.LessonName
                        },
                        AnswerQuestions = q.AnswerQuestions.Select(a => new
                        {
                            a.AnswerQuestionId,
                            a.AnswerText,
                            a.Number
                        }).ToList()
                    })
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Đã xảy ra lỗi khi lấy câu hỏi.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> GetQuestionByIdAsync(int questionId)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);

            if (question == null)
            {
                return new NotFoundObjectResult(new { message = "Không tìm thấy câu hỏi." });
            }

            var response = new
            {
                question.QuestionId,
                question.QuestionText,
                question.CreateAt,
                question.CorrectAnswer,
                question.IsVisible,
                question.CreateByAI,
                Level = new
                {
                    question.Level.LevelName
                },
                Lesson = new
                {
                    question.Lesson.LessonName
                },
                AnswerQuestions = question.AnswerQuestions.Select(a => new
                {
                    a.AnswerQuestionId,
                    a.AnswerText,
                    a.Number
                }).ToList() 
            };

            return new OkObjectResult(response);
        }

        public async Task<IActionResult> UpdateQuestionAsync(int questionId, QuestionDto questionDto)
        {
            try
            {
                var existingQuestion = await _questionRepository.GetQuestionByIdAsync(questionId);
                if (existingQuestion == null)
                {
                    return new NotFoundObjectResult(new { message = "Không tìm thấy câu hỏi." });
                }

           
                if (questionDto.AnswerQuestions == null || questionDto.AnswerQuestions.Count != existingQuestion.AnswerQuestions.Count)
                {
                    return new BadRequestObjectResult(new { message = "Không thể thêm hoặc xóa câu trả lời. Vui lòng cung cấp đúng số lượng câu trả lời hiện có." });
                }

 
                existingQuestion.QuestionText = questionDto.QuestionText;
                existingQuestion.LevelId = questionDto.LevelId;
                existingQuestion.CorrectAnswer = questionDto.CorrectAnswer;
                existingQuestion.IsVisible = questionDto.IsVisible;

                foreach (var answerDto in questionDto.AnswerQuestions)
                {
                    var existingAnswer = existingQuestion.AnswerQuestions
                        .FirstOrDefault(a => a.AnswerQuestionId == answerDto.AnswerQuestionId);

                    if (existingAnswer != null)
                    {
                        existingAnswer.AnswerText = answerDto.AnswerText;
                    }
                    else
                    {
                        return new BadRequestObjectResult(new { message = "Không thể thêm câu trả lời mới khi cập nhật." });
                    }
                }

                await _questionRepository.UpdateQuestionAsync(existingQuestion);

                return new OkObjectResult(new { message = "Cập nhật câu hỏi và danh sách câu trả lời thành công." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Đã xảy ra lỗi khi cập nhật câu hỏi.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }


        public async Task<IActionResult> AddQuestionAsync(CreateQuestionDto questionDto)
        {
            try
            {
                var newQuestion = new Question
                {
                    QuestionText = questionDto.QuestionText,
                    CreateAt = questionDto.CreateAt,
                    LevelId = questionDto.LevelId,
                    CorrectAnswer = questionDto.CorrectAnswer,
                    IsVisible = questionDto.IsVisible,
                    CreateByAI = questionDto.CreateByAI,
                    LessonId = questionDto.LessonId
                };

                var questionId = await _questionRepository.AddQuestionAsync(newQuestion);

  
                if (questionDto.AnswerQuestions != null && questionDto.AnswerQuestions.Any())
                {
                    var answers = questionDto.AnswerQuestions.Select((a, index) => new AnswerQuestion
                    {
                        QuestionId = questionId,
                        AnswerText = a.AnswerText,
                        Number = a.Number 
                    }).ToList();

                    await _questionRepository.AddAnswerQuestionsAsync(answers);
                }

                return new OkObjectResult(new { message = "Câu hỏi và câu trả lời đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Đã xảy ra lỗi khi thêm câu hỏi.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> GenQuestionAIForPractice(QuestionRequest questionRequest)
        {
            int lessonId = Convert.ToInt32(questionRequest.Question);
            var lesson = await _curriculumRepository.GetLessonByIdAsync(lessonId);
            try
            {
                var jsonContent = JsonConvert.SerializeObject(new
                {
                    question = lesson.LessonName,
                    num_questions = questionRequest.NumQuestions
                });

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("http://127.0.0.1:5000/generate-mcq", httpContent);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonResponse = JsonConvert.DeserializeObject<object>(responseBody);
                return new JsonResult(jsonResponse);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Đã xảy ra lỗi khi thêm câu hỏi.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }




    }
}
