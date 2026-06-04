using Microsoft.Extensions.Hosting;

namespace FanaCRM.Services
{
    public class LeadStaleWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public LeadStaleWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<LeadAutomationService>();

                    await service.MarkStaleLeads(7);
                }

                // Run once per day (24h)
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}