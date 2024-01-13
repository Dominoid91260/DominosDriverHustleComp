using DominosDriverHustleComp.Data;
using DominosDriverHustleComp.Models;

using LaunchDarkly.EventSource;

using System.Text.Json;

namespace DominosDriverHustleComp.Services
{
    public class GPSService : IHostedService
    {
        private readonly EventSource _sseClient;
        private readonly ILogger<GPSService> _logger;
        private readonly IServiceProvider _serviceProvider;

        static private readonly JsonSerializerOptions serializerOptions;

        static GPSService()
        {
            serializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public GPSService(ILogger<GPSService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            var conf = Configuration.Builder(new Uri("https://gps-prod-das.dominos.com.au/driver-app-service/dashboard/98037/events"));

            var envAuthKey = Environment.GetEnvironmentVariable("AUTHORIZATION_TOKEN");
            if (envAuthKey != null)
            {
                conf.RequestHeader("authorization", envAuthKey);
            }

            conf.RequestHeaders(new Dictionary<string, string>(){
                { "dpz-market", "AUSTRALIA" },
                { "dpz-language", "en" },
            });

            _sseClient = new EventSource(conf.Build());
            _sseClient.MessageReceived += async (sender, e) => await HandleEvent(e);
            _sseClient.Error += (sender, e) => {
                _logger.LogError("{message}", e.Exception.Message);
                _sseClient.Close();
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _sseClient.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _sseClient.Close();
            return Task.CompletedTask;
        }

        private async Task HandleEvent(MessageReceivedEventArgs e)
        {
            _logger.LogTrace("{name} | {data}", e.EventName, e.Message.Data);

            if (e.EventName != "DRIVER_UPDATED")
                return;

            var update = JsonSerializer.Deserialize<DriverUpdate>(e.Message.Data, serializerOptions);

            if (
                update == null ||
                !update.HeightenedTimeAwareness.DispatchAt.HasValue ||
                !update.HeightenedTimeAwareness.LeftStoreAt.HasValue ||
                !update.HeightenedTimeAwareness.StoreEnterAt.HasValue ||
                !update.HeightenedTimeAwareness.InAt.HasValue)
                return;

            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();

            var driver = await context.Drivers.FindAsync(update.DriverId);
            if (driver == null)
            {
                var entry = await context.Drivers.AddAsync(new()
                {
                    Id = update.DriverId,
                    FirstName = update.FirstName,
                    LastName = update.LastName,
                    Deliveries = new List<DeliveryRecord>()
                });

                driver = entry.Entity;

                await context.SaveChangesAsync();
            }

            await context.Deliveries.AddAsync(new()
            {
                Driver = driver,
                DispatchedAt = update.HeightenedTimeAwareness.DispatchAt.Value,
                LeftStoreAt = update.HeightenedTimeAwareness.LeftStoreAt.Value,
                StoreEnteredAt = update.HeightenedTimeAwareness.StoreEnterAt.Value,
                InAt = update.HeightenedTimeAwareness.InAt.Value,
            });

            await context.SaveChangesAsync();
        }
    }
}
