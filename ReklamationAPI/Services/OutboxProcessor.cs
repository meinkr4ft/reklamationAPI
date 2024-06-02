using Microsoft.EntityFrameworkCore;
using ReklamationAPI.data;

namespace ReklamationAPI.Services
{
    /// <summary>
    /// Service for processing messages in the outbox message table.
    /// </summary>
    public class OutboxProcessor(IServiceProvider serviceProvider, EmailService emailService) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly EmailService _emailService = emailService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var unprocessedMessages = await dbContext.OutboxMessages
                        .Where(m => !m.Processed)
                        .ToListAsync(stoppingToken);

                    foreach (var message in unprocessedMessages)
                    {
                        await _emailService.SendEmailAsync(message);
                        message.Processed = true;
                        dbContext.OutboxMessages.Update(message);
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
