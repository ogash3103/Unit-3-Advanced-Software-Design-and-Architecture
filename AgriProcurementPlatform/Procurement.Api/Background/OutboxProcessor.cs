using Microsoft.EntityFrameworkCore;
using Procurement.Infrastructure.Persistence;

namespace Procurement.Api.Background;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceProvider sp, ILogger<OutboxProcessor> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ProcurementDbContext>();

                var batch = await db.Outbox
                    .Where(x => x.ProcessedAtUtc == null)
                    .OrderBy(x => x.OccurredAtUtc)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                foreach (var msg in batch)
                {
                    try
                    {
                        // Hozircha “publish” o‘rniga log qilamiz.
                        // Keyingi bosqichda RabbitMQ/MassTransit ga yuboramiz.
                        _logger.LogInformation("OUTBOX event: {Type} at {At}", msg.Type, msg.OccurredAtUtc);
                        msg.ProcessedAtUtc = DateTime.UtcNow;
                        msg.Error = null;
                    }
                    catch (Exception ex)
                    {
                        msg.Error = ex.Message;
                    }
                }

                if (batch.Count > 0)
                    await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxProcessor failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
