using DominosDriverHustleComp.Models;

using LaunchDarkly.EventSource;

using System.Text.Json;

namespace DominosDriverHustleComp.Services
{
    public class GPSService : IHostedService
    {
        private readonly EventSource _sseClient;
        private readonly ILogger<GPSService> _logger;
        static private readonly JsonSerializerOptions serializerOptions;

        static GPSService()
        {
            serializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public GPSService(ILogger<GPSService> logger)
        {
            _logger = logger;

            var conf = Configuration.Builder(new Uri("https://gps-prod-das.dominos.com.au/driver-app-service/dashboard/98037/events"));

            var envApiKey = Environment.GetEnvironmentVariable("DPZ_API_KEY");
            if (envApiKey != null)
            {
                conf.RequestHeader("dpz-api-key", envApiKey);
            }

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
            _sseClient.MessageReceived += (sender, e) => HandleEvent(e);
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

        private void HandleEvent(MessageReceivedEventArgs e)
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

            ///@TODO store data
        }
    }
}
