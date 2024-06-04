
using DominosDriverHustleComp.Data;
using DominosDriverHustleComp.Models;

using System.Text.Json;

namespace DominosDriverHustleComp.Services
{
    public class DummyGPSService : IHostedService
    {
        private readonly ILogger<DummyGPSService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DeliveryTrackerService _deliveryTracker;

        static private readonly JsonSerializerOptions serializerOptions;

        static DummyGPSService()
        {
            serializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public DummyGPSService(ILogger<DummyGPSService> logger, IServiceProvider serviceProvider, DeliveryTrackerService deliveryTracker)
        {
            _deliveryTracker = deliveryTracker;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var stream = new StreamReader("data.json");
            string? line;

            while ((line = stream.ReadLine()) != null)
            {
                HandleEvent(line);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void HandleEvent(string Data)
        {
            var update = JsonSerializer.Deserialize<DriverUpdate>(Data, serializerOptions);

            if (update == null)
                return;

            _deliveryTracker.UpdateDriverStatus(update.DriverId, update.DriverStatus, update.DriverStatusAt);

            if (update.DriverStatus == DriverStatus.In || update.HeightenedTimeAwareness.InAt.HasValue)
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();
                var driver = context.Drivers.Find(update.DriverId);

                _logger.LogWarning("Finishing delivery by driver {first} {last}", driver?.FirstName, driver?.LastName);

                _deliveryTracker.FinalizeDeliveryForDriver(update.DriverId, update.HeightenedTimeAwareness);
            }
        }
        }
}
