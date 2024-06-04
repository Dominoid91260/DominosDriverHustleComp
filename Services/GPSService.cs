using DominosDriverHustleComp.Data;
using DominosDriverHustleComp.Models;

using LaunchDarkly.EventSource;

using System.Text.Json;

namespace DominosDriverHustleComp.Services
{
    public class GPSService : BackgroundService
    {
        public string? BearerToken = null;

        private readonly ILogger<GPSService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DeliveryTrackerService _deliveryTracker;

        static private readonly JsonSerializerOptions serializerOptions;

        static GPSService()
        {
            serializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public GPSService(ILogger<GPSService> logger, IServiceProvider serviceProvider, DeliveryTrackerService deliveryTracker)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _deliveryTracker = deliveryTracker;
        }

        private async Task HandleEvent(MessageReceivedEventArgs e)
        {
            // Filter events
            if (e.EventName != "DRIVER_UPDATED")
                return;

            _logger.LogInformation("{data}", e.Message.Data);

            // Deserialize
            var update = JsonSerializer.Deserialize<DriverUpdate>(e.Message.Data, serializerOptions);

            // If theres bad data, dont do anything
            if (update == null)
            {
                _logger.LogError("Deserialized data is null");
                return;
            }

            // Update the delivery tracker
            _deliveryTracker.UpdateDriverStatus(update.DriverId, update.DriverStatus, update.DriverStatusAt);

            // Create a new scope and get the database context
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();

            // Add the driver if they dont exist
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

            if (driver == null)
            {
                _logger.LogError("Driver was null. {code} {first} {last}", update.DriverId, update.FirstName, update.LastName);
                return;
            }

            // Finish the derlivery in the tracker if the driver has signed in
            if (update.DriverStatus == DriverStatus.In || update.HeightenedTimeAwareness.InAt.HasValue)
            {
                _logger.LogWarning("\nFinishing delivery by driver {first} {last}", driver.FirstName, driver.LastName);

                _deliveryTracker.FinalizeDeliveryForDriver(update.DriverId, update.HeightenedTimeAwareness);
            }

            // If theres no dispatch or in times, dont add the delivery to the database
            if (
                !update.HeightenedTimeAwareness.DispatchAt.HasValue ||
                !update.HeightenedTimeAwareness.InAt.HasValue)
                return;

            _logger.LogInformation("Adding to database");

            // Add the delivery to the database
            await context.Deliveries.AddAsync(new()
            {
                Driver = driver,
                DispatchedAt = update.HeightenedTimeAwareness.DispatchAt.Value,
                LeftStoreAt = update.HeightenedTimeAwareness.LeftStoreAt,
                StoreEnteredAt = update.HeightenedTimeAwareness.StoreEnterAt,
                InAt = update.HeightenedTimeAwareness.InAt.Value,
            });

            await context.SaveChangesAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (BearerToken == null)
            {
                _logger.LogInformation("Bearer token is null, waiting 5 seconds");
                await Task.Delay(5000, stoppingToken);
            }

            var conf = Configuration.Builder(new Uri("https://gps-prod-das.dominos.com.au/driver-app-service/dashboard/98037/events"));

            conf.RequestHeaders(new Dictionary<string, string>(){
                { "dpz-market", "AUSTRALIA" },
                { "dpz-language", "en" },
                { "authorization", BearerToken }
            });

            var sseClient = new EventSource(conf.Build());
            sseClient.MessageReceived += async (sender, e) => await HandleEvent(e);
            sseClient.Error += (sender, e) => {
                _logger.LogError("{message}", e.Exception.Message);
                sseClient.Close();
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                await sseClient.StartAsync();
            }
        }
    }
}
