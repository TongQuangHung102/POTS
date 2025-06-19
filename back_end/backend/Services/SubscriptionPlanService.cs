using backend.Dtos.Subscriptions;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _spRepository;

        public SubscriptionPlanService(ISubscriptionPlanRepository spRepository)
        {
            _spRepository = spRepository;
        }

        public async Task AddSubscriptionPlanAsync(SubscriptionPlan plan)
        {
            var existingPlan = await _spRepository.GetByNameAsync(plan.PlanName);
            if (string.IsNullOrWhiteSpace(plan.PlanName))
            {
                throw new InvalidOperationException("Tên gói đăng ký không được để trống.");
            }
            if (existingPlan != null)
            {
                throw new InvalidOperationException("Tên gói đăng ký đã tồn tại. Vui lòng chọn tên khác.");
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
                IsVisible = plan.IsVisible
            }).ToList();
        }
        public async Task<bool> UpdatePlanAsync(int id, SubscriptionPlanDto dto)
        {
            var existingPlan = await _spRepository.GetByIdAsync(id);

            if (existingPlan == null)
            {
                return false;
            }

            var planWithSameName = await _spRepository.GetByNameAsync(dto.PlanName);
            if (planWithSameName != null && planWithSameName.PlanId != id)
            {
                throw new InvalidOperationException("Tên gói đăng ký đã tồn tại. Vui lòng chọn tên khác.");
            }

            existingPlan.PlanName = dto.PlanName;
            existingPlan.Price = dto.Price;
            existingPlan.Description = dto.Description;
            existingPlan.Duration = dto.Duration;
            existingPlan.MaxExercisesPerDay = dto.MaxExercisesPerDay;
            existingPlan.IsVisible = dto.IsVisible;
            existingPlan.UpdatedAt = DateTime.UtcNow.AddHours(7);

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
                CreatedAt = plan.CreatedAt,  
                UpdatedAt = plan.UpdatedAt   
            };
        }

    }
}
