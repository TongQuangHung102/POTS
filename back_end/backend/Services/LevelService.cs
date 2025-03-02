using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class LevelService
    {
        private readonly ILevelRepository _lvRepository;

        public LevelService(ILevelRepository lvRepository)
        {
            _lvRepository = lvRepository;
        }

        public async Task<IActionResult> GetAllLevel()
        {
            try
            {
                var level = await _lvRepository.getAllLevel();
                if (level == null)
                {
                    return new OkObjectResult(new { Message = "Không có level nào trong hệ thống." });
                }

                return new OkObjectResult(level);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Đã xảy ra lỗi khi lấy danh sách.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
    }
}
