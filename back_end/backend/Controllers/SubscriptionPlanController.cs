using backend.DataAccess.DAO;
using backend.Dtos.Subscriptions;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly SubscriptionPlanService _spService;

        public SubscriptionPlanController(SubscriptionPlanService spService)
        {
            _spService = spService;
        }

        [HttpGet("get-all-subscriptionplan")]
        public async Task<ActionResult<List<SubscriptionPlanDto>>> GetAllPlans()
        {
            var plans = await _spService.GetAllPlansAsync();
            return Ok(plans);
        }

        [HttpPost("add-subscriptionplan")]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] SubscriptionPlanDto dto)
        {
            try
            {
                var plan = new SubscriptionPlan
                {
                    PlanName = dto.PlanName,
                    Price = dto.Price,
                    Description = dto.Description,
                    Duration = dto.Duration, 
                    MaxExercisesPerDay = dto.MaxExercisesPerDay,
                    IsAIAnalysis = dto.IsAIAnalysis,
                    IsPersonalization = dto.IsPersonalization,
                    IsBasicStatistics = dto.IsBasicStatistics,
                    IsAdvancedStatistics = dto.IsAdvancedStatistics,
                    CreatedAt = DateTime.UtcNow
                };

                await _spService.AddSubscriptionPlanAsync(plan);

                return Ok(new { message = "Subscription Plan created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("edit-subscriptionplan/{id}")]
        public async Task<ActionResult> UpdatePlan(int id, [FromBody] SubscriptionPlanDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _spService.UpdatePlanAsync(id, dto);

            if (!result)
            {
                return NotFound(new { message = "Subscription plan not found." });
            }

            return Ok(new { message = "Subscription plan updated successfully." });
        }

        [HttpGet("get-subscriptionplan-by/{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> GetPlanDetail(int id)
        {
            var planDetail = await _spService.GetPlanDetailAsync(id);

            if (planDetail == null)
            {
                return NotFound(new { message = "Subscription plan not found." });
            }

            return Ok(planDetail);
        }
    }
}
