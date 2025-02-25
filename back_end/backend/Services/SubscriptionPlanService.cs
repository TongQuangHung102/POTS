using backend.Dtos;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class SubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _spRepository;

        public SubscriptionPlanService(ISubscriptionPlanRepository spRepository)
        {
            _spRepository = spRepository;
        }

        public async Task AddSubscriptionPlanAsync(SubscriptionPlan plan)
        {
            var existingPlan = await _spRepository.GetByNameAsync(plan.PlanName);
            if (existingPlan != null)
            {
                throw new Exception("Tên gói đăng ký đã tồn tại. Vui lòng chọn tên khác.");
            }
            await _spRepository.AddSubscriptionPlanAsync(plan);
        }

        public async Task<List<SubscriptionPlanDto>> GetAllPlansAsync()
        {
            var plans = await _spRepository.GetAllAsync();
            return plans.Select(plan => new SubscriptionPlanDto
            {
                PlanId = plan.PlanId,
                PlanName = plan.PlanName,
                Price = plan.Price,
                Description = plan.Description,
                Duration = plan.Duration, 
                MaxExercisesPerDay = plan.MaxExercisesPerDay,
                IsAIAnalysis = plan.IsAIAnalysis,
                IsPersonalization = plan.IsPersonalization,
                IsBasicStatistics = plan.IsBasicStatistics,
                IsAdvancedStatistics = plan.IsAdvancedStatistics
            }).ToList();
        }
        public async Task<bool> UpdatePlanAsync(int id, SubscriptionPlanDto dto)
        {
            var existingPlan = await _spRepository.GetByIdAsync(id);

            if (existingPlan == null)
            {
                return false; 
            }

            existingPlan.PlanName = dto.PlanName;
            existingPlan.Price = dto.Price;
            existingPlan.Description = dto.Description;
            existingPlan.Duration = dto.Duration;
            existingPlan.MaxExercisesPerDay = dto.MaxExercisesPerDay;
            existingPlan.IsAIAnalysis = dto.IsAIAnalysis;
            existingPlan.IsPersonalization = dto.IsPersonalization;
            existingPlan.IsBasicStatistics = dto.IsBasicStatistics;
            existingPlan.IsAdvancedStatistics = dto.IsAdvancedStatistics;
            existingPlan.UpdatedAt = DateTime.UtcNow;

            await _spRepository.UpdateAsync(existingPlan);
            return true;
        }
        public async Task<SubscriptionPlanDto> GetPlanDetailAsync(int id)
        {
            var plan = await _spRepository.GetByIdAsync(id);

            if (plan == null)
            {
                return null;  
            }

            return new SubscriptionPlanDto
            {
                PlanId = plan.PlanId,
                PlanName = plan.PlanName,
                Price = plan.Price,
                Description = plan.Description,
                Duration = plan.Duration,
                MaxExercisesPerDay = plan.MaxExercisesPerDay,
                IsAIAnalysis = plan.IsAIAnalysis,
                IsPersonalization = plan.IsPersonalization,
                IsBasicStatistics = plan.IsBasicStatistics,
                IsAdvancedStatistics = plan.IsAdvancedStatistics,
                CreatedAt = plan.CreatedAt,  
                UpdatedAt = plan.UpdatedAt   
            };
        }

    }
}
