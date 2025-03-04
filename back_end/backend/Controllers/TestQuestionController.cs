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


        [HttpGet("get-questions-by-test/{testId}")]
        public async Task<ActionResult<List<TestQuestionDto>>> GetTestQuestions(int testId)
        {
            var result = await _testQuestionService.GetTestQuestionsByTestId(testId);
            return Ok(result);
        }
    }
}
