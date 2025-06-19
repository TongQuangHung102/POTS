using backend.Dtos.StudentAnswer;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAnswerController : Controller
    {
        private readonly StudentAnswersService _studentAnswerService;

        public StudentAnswerController(StudentAnswersService studentAnswerService)
        {
            _studentAnswerService = studentAnswerService;
        }
    }
}
