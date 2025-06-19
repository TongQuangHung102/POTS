using backend.Services;

namespace backend.BackgroundTasks
{
    public class CleanupOldPracticeAttemptsService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CleanupOldPracticeAttemptsService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var practiceService = scope.ServiceProvider.GetRequiredService<PracticeAttemptService>();
                    await practiceService.DeleteOldPracticeAttemptsAsync();

                    var questionsPracticeService = scope.ServiceProvider.GetRequiredService<PracticeQuestionService>();
                    await questionsPracticeService.CleanupUnansweredPracticeQuestionsAsync();
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
