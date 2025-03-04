using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestQuestionController : ControllerBase
    {
        private readonly TestQuestionService _service;

        public TestQuestionController(TestQuestionService service)
        {
            _service = service;
        }

        [HttpPost("add-questions")]
        public async Task<IActionResult> AddQuestionsToTest([FromBody] AddQuestionsToTestDto dto)
        {
          return await _service.AddQuestionsToTest(dto);
        }

        [HttpGet("get-test-questions")]
        public async Task<IActionResult> GetTestQuestions(int testId)
        {
             return await _service.GetTestQuestionsAsync(testId);
            
        }

        [HttpPut("update-questions")]
        public async Task<IActionResult> UpdateTestQuestions([FromBody] AddQuestionsToTestDto dto)
        {
           return await _service.UpdateTestQuestionsAsync(dto);
        }

    }
}
