using DominosDriverHustleComp.Server.Models.GPS;
using LaunchDarkly.EventSource;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DominosDriverHustleComp.Server.Services
{
    public class GPSSSEService : BackgroundService
    {
        private readonly ILogger<GPSSSEService> _logger;
        private readonly HustleTracker _hustleTracker;
        private EventSource? _sseClient;
        private JsonSerializerOptions _serializerOptions;

        public string? BearerToken { get; set; }

        public GPSSSEService(ILogger<GPSSSEService> logger, HustleTracker hustleTracker)
        {
            _logger = logger;
            _hustleTracker = hustleTracker;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            /// API KEYS https://gps-dashboard.dominos.com.au/return-driver-dashboard/_envconfig.js

            await WaitForAuth(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            _logger.LogInformation("Setting up SSE client");

            var conf = Configuration.Builder(new Uri("https://gps-prod-das.dominos.com.au/driver-app-service/dashboard/98037/events"));
            conf.RequestHeaders(new Dictionary<string, string>(){
                { "dpz-market", "AUSTRALIA" },
                { "dpz-language", "en" },
                { "authorization", BearerToken! }
            });

            _sseClient = new EventSource(conf.Build());
            _sseClient.MessageReceived += (s, e) => HandleEvent(e.EventName, e.Message.Data);
            _sseClient.Error += (s, e) => HandleError(e.Exception.Message);
            await _sseClient.StartAsync();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
            }

            _sseClient?.Close();
        }

        private async Task WaitForAuth(CancellationToken cancellationToken)
        {
            // If there is no bearer token or no api key check again every 5 seconds
            while (!cancellationToken.IsCancellationRequested && BearerToken is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        private void HandleError(string errorMessage)
        {
            _logger.LogError("Error, resetting token: {message}", errorMessage);
            BearerToken = null;
        }

        private void HandleEvent(string eventName, string data)
        {
            if (eventName == "DRIVERS_CURRENT_STATE")
            {
                var updates = JsonSerializer.Deserialize<IEnumerable<DriverUpdate>>(data, _serializerOptions);
                foreach (var update in updates)
                {
                    ProcessDriverUpdate(update);
                }
            }
            else if (eventName == "DRIVER_UPDATED")
            {
                var update = JsonSerializer.Deserialize<DriverUpdate>(data, _serializerOptions);
                ProcessDriverUpdate(update);
            }
            else if (eventName == "STORE_UPDATED")
            {
                var update = JsonSerializer.Deserialize<StoreUpdate>(data, _serializerOptions);
                ///@TODO implement
            }
        }

        private void ProcessDriverUpdate(DriverUpdate update)
        {
            if (update == null ||
                update.DriverStatus != DriverStatus.In ||
                update.HeightenedTimeAwareness is null)
                return;

            _hustleTracker.StoreHustleData(update);
        }
    }
}
