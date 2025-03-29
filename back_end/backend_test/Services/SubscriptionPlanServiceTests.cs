using backend.Dtos.Subscriptions;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace backend_test.Services
{
    public class SubscriptionPlanServiceTests
    {
        private readonly Mock<ISubscriptionPlanRepository> _mockRepo;
        private readonly SubscriptionPlanService _service;

        public SubscriptionPlanServiceTests()
        {
            _mockRepo = new Mock<ISubscriptionPlanRepository>();
            _service = new SubscriptionPlanService(_mockRepo.Object);
        }

        [Theory]
        [InlineData("Nâng cao", true)]
        [InlineData("Cơ bản", false)] 
        [InlineData("", false)] 
        [InlineData(null, false)]
        public async Task AddSubscriptionPlanAsync_ShouldHandleDuplicateNames(string planName, bool shouldSucceed)
        {
            var existingPlan = planName == "Cơ bản" ? new SubscriptionPlan { PlanName = "Cơ bản" } : null;

            _mockRepo.Setup(repo => repo.GetByNameAsync(planName)).ReturnsAsync(existingPlan);

            if (shouldSucceed)
            {
                await _service.AddSubscriptionPlanAsync(new SubscriptionPlan { PlanName = planName });
                _mockRepo.Verify(repo => repo.AddSubscriptionPlanAsync(It.IsAny<SubscriptionPlan>()), Times.Once);
            }
            else
            {
                await Assert.ThrowsAsync<Exception>(() => _service.AddSubscriptionPlanAsync(new SubscriptionPlan { PlanName = planName }));
            }
        }

        [Theory]
        [InlineData(1, "Nâng cao", 15, true)]
        [InlineData(0, "Nâng cao", 25, false)]
        public async Task UpdatePlanAsync_ShouldHandlePlanUpdate(int planId, string newName, decimal newPrice, bool shouldSucceed)
        {
            var existingPlan = planId > 0 ? new SubscriptionPlan { PlanId = planId, PlanName = "Cơ bản", Price = 10 } : null;

            _mockRepo.Setup(repo => repo.GetByIdAsync(planId)).ReturnsAsync(existingPlan);
            _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<SubscriptionPlan>())).Returns(Task.CompletedTask);

            var dto = new SubscriptionPlanDto { PlanName = newName, Price = newPrice };

            if (shouldSucceed)
            {
                var result = await _service.UpdatePlanAsync(planId, dto);
                Assert.True(result);
                _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<SubscriptionPlan>()), Times.Once);
            }
            else
            {
                var result = await _service.UpdatePlanAsync(planId, dto);
                Assert.False(result);
            }
        }
    }
}
