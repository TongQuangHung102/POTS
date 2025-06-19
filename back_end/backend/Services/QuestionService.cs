using backend.DataAccess.DAO;
using backend.Dtos.AIQuestions;
using backend.Dtos.Curriculum;
using backend.Dtos.Dashboard;
using backend.Dtos.Questions;
using backend.Hubs;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
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
        private readonly IPracticeQuestionRepository _practiceQuestionRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserParentStudentRepository _userParentStudentRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserRepository _userRepository;

        public QuestionService(IQuestionRepository questionRepository, 
            ICurriculumRepository curriculumRepository, 
            IPracticeRepository practiceRepository, 
            HttpClient httpClient, 
            IPracticeQuestionRepository practiceQuestionRepository, 
            INotificationRepository notificationRepository, 
            IUserParentStudentRepository userParentStudentRepository, 
            IHubContext<NotificationHub> hubContext, 
            IUserRepository userRepository)
        {
            _questionRepository = questionRepository;
            _curriculumRepository = curriculumRepository;
            _practiceRepository = practiceRepository;
            _httpClient = httpClient;
            _practiceQuestionRepository = practiceQuestionRepository;
            _notificationRepository = notificationRepository;
            _userParentStudentRepository = userParentStudentRepository;
            _hubContext = hubContext;
            _userRepository = userRepository;
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

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                throw new KeyNotFoundException("Không tìm thấy câu hỏi.");
            }

            return question;
        }


        public async Task UpdateQuestionAsync(int questionId, QuestionDto questionDto)
        {
            if (questionDto == null)
            {
                throw new ArgumentNullException(nameof(questionDto), "Dữ liệu câu hỏi không hợp lệ.");
            }

            var existingQuestion = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (existingQuestion == null)
            {
                throw new KeyNotFoundException("Không tìm thấy câu hỏi.");
            }

            if (questionDto.AnswerQuestions == null || questionDto.AnswerQuestions.Count != existingQuestion.AnswerQuestions.Count)
            {
                throw new InvalidOperationException("Không thể thêm hoặc xóa câu trả lời. Vui lòng cung cấp đúng số lượng câu trả lời hiện có.");
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
                    throw new InvalidOperationException("Không thể thêm câu trả lời mới khi cập nhật.");
                }
            }

            await _questionRepository.UpdateQuestionAsync(existingQuestion);
        }


        public async Task AddQuestionAsync(CreateQuestionDto questionDto)
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
                    var answers = questionDto.AnswerQuestions.Select(a => new AnswerQuestion
                    {
                        QuestionId = questionId,
                        AnswerText = a.AnswerText,
                        Number = a.Number
                    }).ToList();

                    await _questionRepository.AddAnswerQuestionsAsync(answers);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm câu hỏi.", ex);
            }
        }

        public async Task<(List<Question>, bool byAi, bool firstTime)> GenQuestionAIForPractice(QuestionRequest questionRequest, bool? IsTrial, DateTime? ExpirationDate, int? maxExercises = null)
        {
            var canPractice = await CanPracticeAsync(questionRequest.userId, IsTrial, ExpirationDate, maxExercises);
            if (!canPractice)
            {
                await NotifyParentsStudentWantsToPracticeAsync(questionRequest.userId);
                throw new Exception("Bạn đã hết số lần luyện tập, hãy quay lại vào ngày mai hoặc nâng cấp gói.");
            }
            int score = 0;
            var qIds = new List<int>();

            try
            {
                var attempt = await _practiceRepository.GetLatestAttemptAsync(questionRequest.userId, questionRequest.lessonId);
               
                if (attempt != null)
                {
                    var lesson = await _curriculumRepository.GetAllInfoLessonByIdAsync(questionRequest.lessonId);
                    score = attempt.CorrectAnswers;
                    var random = new Random();
                   

                    var randomQuestions = attempt.StudentAnswers
                                        .Select(sa => sa.Question != null ? new { Id = sa.Question.QuestionId, Text = sa.Question.QuestionText }
                                        : new { Id = sa.PracticeQuestion.PracticeQuestionId, Text = sa.PracticeQuestion.QuestionText })
                                        .GroupBy(q => q.Id)
                                        .Select(g => g.First())
                                        .OrderBy(q => random.Next())
                                        .Select(q => q.Text)
                                        .Take(5)
                                        .ToList();

                    var joinedQuestions = string.Join(", ", randomQuestions);
                    var jsonContent = JsonConvert.SerializeObject(new PracticeSession
                    {
                        subject = "Bài " + lesson.Order + " :" + lesson.LessonName,
                        question = joinedQuestions,
                        num_questions = 10,
                        collection_name = lesson.Chapter.SubjectGrade.CollectionName + lesson.Chapter.Semester.ToString(),
                        results = new PracticeResults
                        {
                            num_correct = attempt.CorrectAnswers,
                            total_questions = 10,
                            time_taken = attempt.TimePractice
                        }
                    });

                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    Console.WriteLine(await httpContent.ReadAsStringAsync());


                    HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:5000/generate-adaptive-question", httpContent);
                   // HttpResponseMessage response = await _httpClient.PostAsync("http://ai:5000/generate-adaptive-question", httpContent);

                    if (!response.IsSuccessStatusCode)
                    {
                         throw new HttpRequestException($"Lỗi gọi API AI: {response.StatusCode}");
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody) || responseBody == "[]")
                    {
                        throw new Exception("API trả về dữ liệu rỗng hoặc không hợp lệ.");
                    }
                    Console.WriteLine("Du lieu tra ve" + responseBody);


                    var aiResponse = JsonConvert.DeserializeObject<AIQuestionResponse>(responseBody);

                    if (aiResponse?.Questions == null || !aiResponse.Questions.Any())
                    {
                        throw new Exception("AI API trả về danh sách rỗng hoặc không hợp lệ.");
                    }

                    var questions = new List<Question>();

                    foreach (var question in aiResponse.Questions)
                    {
                        var practiceQuestion = new PracticeQuestion
                        {
                            QuestionText = question.QuestionText,
                            CorrectAnswer = question.CorrectAnswer,
                            LessonId = questionRequest.lessonId,
                            CreateAt = DateTime.UtcNow.AddHours(7)
                        };

                        var pQ = await _practiceQuestionRepository.CreateAsync(practiceQuestion);

                        var answerPracticeQuestion = question.AnswerQuestions.Select(n => new AnswerPracticeQuestion
                        {
                            AnswerText = n.AnswerText,
                            Number = n.Number,
                            PracticeQuestionId = pQ.PracticeQuestionId
                        });

                        await _practiceQuestionRepository.AddAnswerQuestionsAsync(answerPracticeQuestion.ToList());

                        questions.Add(new Question
                        {
                            QuestionText = question.QuestionText,
                            CorrectAnswer = question.CorrectAnswer,
                            QuestionId = pQ.PracticeQuestionId,
                            AnswerQuestions = question.AnswerQuestions.Select(a => new AnswerQuestion
                            {
                                AnswerText = a.AnswerText,
                                Number = a.Number
                            }).ToList()
                        });
                    }
                    return (questions, true, false);
                }
                else
                {
                    var qs = await _questionRepository.GetQuestionsFirstTimePractice(10, questionRequest.lessonId);
                    foreach (var question in qs)
                    {
                        if (!question.IsUsed)
                        {
                            qIds.Add(question.QuestionId);
                        }
                    }
                    await _questionRepository.MarkQuestionsAsUsed(qIds);
                    return (qs, false, true);
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
            var q = await _questionRepository.GetQuestionsPractice(10, questionRequest.lessonId, GetLevelIdFromScore(score));

            foreach (var question in q)
            {
                if (!question.IsUsed)
                {
                    qIds.Add(question.QuestionId);
                }
            }
            await _questionRepository.MarkQuestionsAsUsed(qIds);
            return (q, false, false);
        }

        public async Task<List<Question>> GenerateTestQuestionsAsync(GenerateTestRequest request)
        {
            if (request == null || request.Chapters == null || request.Chapters.Count == 0)
            {
                throw new ArgumentException("Danh sách chương không được để trống.");
            }

            var questions = new List<Question>();
            var qIds = new List<int>();

            foreach (var chapterRequest in request.Chapters)
            {
                var chapterQuestions = await _questionRepository.GetQuestionsByChapterAutoAsync(chapterRequest);
                questions.AddRange(chapterQuestions);
            }

            if (questions.Count == 0)
            {
                throw new Exception("Không tìm thấy câu hỏi phù hợp.");
            }

            foreach (var question in questions)
            {
                if (!question.IsUsed)
                {
                    qIds.Add(question.QuestionId);
                }
            }
            await _questionRepository.MarkQuestionsAsUsed(qIds);

            return questions;
        }

        public async Task<bool> CanPracticeAsync(int userId, bool? isTrial, DateTime? expirationDate, int? maxExercises)
        {
            if (isTrial == false)
            {
                if (expirationDate != null && expirationDate.Value.Date >= DateTime.UtcNow.AddHours(7).Date)
                {
                    return true;
                }
                return false;
            }

            if (isTrial == true)
            {
                var today = DateTime.UtcNow.AddHours(7).Date;
                var practiceCount = await _practiceRepository.GetTotalNumberAllPracticeAsync(userId, today);

                return maxExercises == null || practiceCount < maxExercises;
            }

            return false;
        }

        private async Task NotifyParentsStudentWantsToPracticeAsync(int studentId)
        {
            var parents = await _userParentStudentRepository.GetParentsByStudentIdAsync(studentId);
            var student = await _userRepository.GetUserByIdAsync(studentId);
            foreach (var parent in parents)
            {
                var title = "Con bạn đang muốn tiếp tục luyện tập!";
                var content = $"Con ({student.UserName}) của bạn đang cố gắng luyện tập dù đã hết lượt trong gói hiện tại. Hãy nâng cấp gói để tiếp thêm động lực cho bé nhé!";

                var noti = new Notification
                {
                    UserId = parent.UserId,
                    Title = title,
                    Content = content,
                    Type = "Thông báo",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _notificationRepository.AddNotificationAsync(noti);
                await _hubContext.Clients.User(parent.UserId.ToString()).SendAsync("ReceiveNotification");
            }
        }




        public int GetLevelIdFromScore(int score)
        {
            if (score <= 5)
                return 1;
            else if (score <= 7)
                return 2;
            else if (score <= 9)
                return 3;
            else
                return 4;
        }
    }
}
