using backend.Dtos.AIQuestions;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace backend_test.Services
{
    public class AIQuestionServiceTests
    {
        [Fact]
        public async Task GenerateAndSaveAIQuestions_ReturnsListOfIds_WhenQuestionsAreGenerated()
        {
            // Arrange
            var requestDto = new AIQuestionRequestDto
            {
                Question = "What is the capital of France?",
                NumQuestions = 1
            };

            var fakeResponseDto = new AIQuestionResponseDto
            {
                Questions = new List<AIQuestionDetailDto>
            {
                new AIQuestionDetailDto
                {
                    QuestionText = "What is 2+2?",
                    LevelId = 1,
                    CorrectAnswer = 1,
                    AnswerQuestions = new List<AnswerDto>
                    {
                        new AnswerDto { AnswerText = "4", Number = 1 },
                        new AnswerDto { AnswerText = "5", Number = 2 }
                    }
                }
            }
            };

            var jsonResponse = JsonConvert.SerializeObject(fakeResponseDto);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:5000")
            };

            var repositoryMock = new Mock<IAIQuestionRepository>();
            repositoryMock
                .Setup(r => r.SaveAIQuestionAsync(It.IsAny<backend.Models.AIQuestion>()))
                .Callback<backend.Models.AIQuestion>(q => q.QuestionId = 123)
                .Returns(Task.CompletedTask);

            repositoryMock
                .Setup(r => r.SaveAnswerAsync(It.IsAny<AnswerQuestion>()))
                .Returns(Task.CompletedTask);

            var service = new AIQuestionService(httpClient, repositoryMock.Object);

            // Act
            var result = await service.GenerateAndSaveAIQuestions(requestDto);

            // Assert
            Assert.Single(result);
            Assert.Equal(123, result.First());
        }


        [Fact]
        public async Task GenerateAndSaveAIQuestions_ReturnsEmptyList_WhenFlaskReturnsError()
        {
            // Arrange
            var requestDto = new AIQuestionRequestDto
            {
                Question = "Test question?",
                NumQuestions = 1
            };

            var httpHandlerMock = new Mock<HttpMessageHandler>();
            httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("")
                });

            var httpClient = new HttpClient(httpHandlerMock.Object);
            var repositoryMock = new Mock<IAIQuestionRepository>();

            var service = new AIQuestionService(httpClient, repositoryMock.Object);

            // Act
            var result = await service.GenerateAndSaveAIQuestions(requestDto);

            // Assert
            Assert.Empty(result);
            repositoryMock.Verify(r => r.SaveAIQuestionAsync(It.IsAny<backend.Models.AIQuestion>()), Times.Never);
        }
        [Fact]
        public async Task GenerateAndSaveAIQuestions_ReturnsEmptyList_WhenNoQuestionsInResponse()
        {
            // Arrange
            var requestDto = new AIQuestionRequestDto { Question = "Empty", NumQuestions = 1 };

            var responseDto = new AIQuestionResponseDto { Questions = new List<AIQuestionDetailDto>() };
            var jsonResponse = JsonConvert.SerializeObject(responseDto);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var repositoryMock = new Mock<IAIQuestionRepository>();
            var service = new AIQuestionService(httpClient, repositoryMock.Object);

            // Act
            var result = await service.GenerateAndSaveAIQuestions(requestDto);

            // Assert
            Assert.Empty(result);
            repositoryMock.Verify(r => r.SaveAIQuestionAsync(It.IsAny<backend.Models.AIQuestion>()), Times.Never);
        }

        [Fact]
        public async Task GenerateAndSaveAIQuestions_SavesMultipleQuestionsAndAnswers()
        {
            // Arrange
            var requestDto = new AIQuestionRequestDto { Question = "Multi", NumQuestions = 2 };

            var responseDto = new AIQuestionResponseDto
            {
                Questions = new List<AIQuestionDetailDto>
        {
            new() {
                QuestionText = "Q1?", LevelId = 1, CorrectAnswer = 1,
                AnswerQuestions = new() {
                    new() { AnswerText = "A1", Number = 1 },
                    new() { AnswerText = "A2", Number = 2 }
                }
            },
            new() {
                QuestionText = "Q2?", LevelId = 2, CorrectAnswer = 2,
                AnswerQuestions = new() {
                    new() { AnswerText = "B1", Number = 1 },
                    new() { AnswerText = "B2", Number = 2 }
                }
            }
        }
            };

            var jsonResponse = JsonConvert.SerializeObject(responseDto);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var repositoryMock = new Mock<IAIQuestionRepository>();

            int questionIdSeed = 100;
            repositoryMock
                .Setup(r => r.SaveAIQuestionAsync(It.IsAny<backend.Models.AIQuestion>()))
                .Callback<backend.Models.AIQuestion>(q => q.QuestionId = questionIdSeed++)
                .Returns(Task.CompletedTask);

            repositoryMock
                .Setup(r => r.SaveAnswerAsync(It.IsAny<AnswerQuestion>()))
                .Returns(Task.CompletedTask);

            var service = new AIQuestionService(httpClient, repositoryMock.Object);

            // Act
            var result = await service.GenerateAndSaveAIQuestions(requestDto);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(100, result);
            Assert.Contains(101, result);

            repositoryMock.Verify(r => r.SaveAIQuestionAsync(It.IsAny<backend.Models.AIQuestion>()), Times.Exactly(2));
            repositoryMock.Verify(r => r.SaveAnswerAsync(It.IsAny<AnswerQuestion>()), Times.Exactly(4)); // 2 answers each
        }

    }


}