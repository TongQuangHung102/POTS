using backend.Services;

namespace backend.BackgroundTasks
{
    public class CheckInactiveStudentsService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CheckInactiveStudentsService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                        await notificationService.SendNotificationsToInactiveStudentsAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi trong Background Service: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }
        }
    }
}
