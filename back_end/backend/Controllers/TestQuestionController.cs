using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestQuestionController : ControllerBase
    {
        private readonly TestQuestionService _testQuestionService;

        public TestQuestionController(TestQuestionService testQuestionService)
        {
            _testQuestionService = testQuestionService;
        }

        [HttpPost("add-questions")]
        public async Task<IActionResult> AddQuestionsToTest([FromBody] AddQuestionsToTestDto dto)
        {
          return await _testQuestionService.AddQuestionsToTest(dto);
        }

        [HttpGet("get-test-questions")]
        public async Task<IActionResult> GetTestQuestions(int testId)
        {
            return await _testQuestionService.GetTestQuestionsAsync(testId);

        }

        [HttpPut("update-questions")]
        public async Task<IActionResult> UpdateTestQuestions([FromBody] AddQuestionsToTestDto dto)
        {
           return await _testQuestionService.UpdateTestQuestionsAsync(dto);
        }

    }
}
