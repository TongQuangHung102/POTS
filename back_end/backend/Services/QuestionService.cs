using backend.DataAccess.DAO;
using backend.Dtos.AIQuestions;
using backend.Dtos.Curriculum;
using backend.Dtos.Dashboard;
using backend.Dtos.Questions;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly IPracticeRepository _practiceRepository;
        private readonly HttpClient _httpClient;

        public QuestionService(IQuestionRepository questionRepository, HttpClient httpClient, ICurriculumRepository curriculumRepository, IPracticeRepository practiceRepository)
        {
            _questionRepository = questionRepository;
            _httpClient = httpClient;
            _curriculumRepository = curriculumRepository;
            _practiceRepository = practiceRepository;
        }

        public async Task<QuestionResponseDto> GetAllQuestionsAsync(int? chapterId, int? lessonId, int? levelId,string searchTerm, bool? isVisible, int page, int pageSize)
        {
                var lesson = new Lesson();
                var totalQuestions = await _questionRepository.GetTotalQuestionsAsync(chapterId, lessonId, levelId, searchTerm, isVisible);
                var questions = await _questionRepository.GetAllQuestionsAsync(chapterId, lessonId, levelId, searchTerm, isVisible, page, pageSize);
                if (lessonId.HasValue)
                {
                    lesson = await _curriculumRepository.GetLessonByIdAsync(lessonId.Value);
                }

                var response = new QuestionResponseDto
                {
                    LessonName = lesson.LessonName,
                    TotalQuestions = totalQuestions,
                    Page = page,
                    PageSize = pageSize,
                    Data = questions.Select(q => new QuestionDetailDto
                    {
                        QuestionId = q.QuestionId,
                        QuestionText = q.QuestionText,
                        CreateAt = q.CreateAt,
                        CorrectAnswer = q.CorrectAnswer,
                        CorrectAnswerText = q.AnswerQuestions.FirstOrDefault(a => a.Number == q.CorrectAnswer)?.AnswerText,
                        IsVisible = q.IsVisible,
                        CreateByAI = q.CreateByAI,
                        Level = new LevelSimpleDto
                        {
                            LevelId = q.Level.LevelId,
                            LevelName = q.Level.LevelName
                        },
                        Lesson = new LessonNameDto
                        {
                            LessonName = q.Lesson.LessonName
                        },
                        AnswerQuestions = q.AnswerQuestions.Select(a => new AnswerQuestionDto
                        {
                            AnswerQuestionId = a.AnswerQuestionId,
                            AnswerText = a.AnswerText,
                            Number = a.Number
                        }).ToList()
                    }).ToList()
                };

            return response;
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

        public async Task<(List<Question>, bool byAi)> GenQuestionAIForPractice(QuestionRequest questionRequest)
        {
            int levelId = 0;
            bool byAi = true;
            try
            {
                var attempt = await _practiceRepository.GetLastAttempt(questionRequest.userId, questionRequest.lessonId);

                if (attempt != null)
                {
                    levelId = attempt.LevelId;

                    var jsonContent = JsonConvert.SerializeObject(new PracticeSession
                    {
                        question = attempt.SampleQuestion,
                        num_questions = 5,
                        results = new PracticeResults
                        {
                            num_correct = attempt.CorrectAnswers,
                            total_questions = 5,
                            time_taken = 300
                        }
                    });

                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    Console.WriteLine(await httpContent.ReadAsStringAsync());


                    HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:5000/generate-question", httpContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Lỗi gọi API AI: {response.StatusCode}");
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Du lieu tra ve" + responseBody);


                    var aiResponse = JsonConvert.DeserializeObject<AIQuestionResponse>(responseBody);

                    if (aiResponse == null || aiResponse.Questions == null)
                    {
                        throw new Exception("AI API trả về danh sách rỗng hoặc không hợp lệ.");
                    }

                    var questions = aiResponse.Questions.Select(q => new Question
                    {
                        QuestionText = q.QuestionText,
                        LevelId = q.LevelId,
                        CorrectAnswer = q.CorrectAnswer,
                        CreateAt = DateTime.Now,
                        IsVisible = true,
                        CreateByAI = true,
                        LessonId = questionRequest.lessonId,
                        AnswerQuestions = q.AnswerQuestions.Select(a => new AnswerQuestion
                        {
                            AnswerText = a.AnswerText,
                            Number = a.Number
                        }).ToList()
                    }).ToList();
                    return (questions, byAi);
                }
                else
                {
                    var q = await _questionRepository.GetQuestionsFirstTimePractice(5, questionRequest.lessonId);
                    return (q, byAi);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Lỗi khi gọi API AI: " + ex.Message);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Lỗi không xác định: " + ex.Message);
            }

            byAi = false;
            return (await _questionRepository.GetQuestionsPractice(5, questionRequest.lessonId, levelId), byAi);
        }

        public async Task<List<Question>> GenerateTestQuestionsAsync(GenerateTestRequest request)
        {
            if (request == null || request.Chapters == null || request.Chapters.Count == 0)
            {
                throw new ArgumentException("Danh sách chương không được để trống.");
            }

            var questions = new List<Question>();

            foreach (var chapterRequest in request.Chapters)
            {
                var chapterQuestions = await _questionRepository.GetQuestionsByChapterAutoAsync(chapterRequest);
                questions.AddRange(chapterQuestions);
            }

            if (questions.Count == 0)
            {
                throw new Exception("Không tìm thấy câu hỏi phù hợp.");
            }

            return questions;
        }


    }
}
