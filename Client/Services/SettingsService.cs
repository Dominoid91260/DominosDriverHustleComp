using DominosDriverHustleComp.Shared.ViewModels;
using System.Net.Http.Json;

namespace DominosDriverHustleComp.Client.Services
{
    public class SettingsService
    {

        private readonly IServiceProvider _serviceProvider;

        public SettingsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public int HustleBenchmarkSeconds { get; set; }
        public int OutlierSeconds { get; set; }
        public int MinDels { get; set; }
        public int MinTrackedPercentage { get; set; }
        public int MaxOverspeeds { get; set; }
        public bool ShowDeliveries { get; set; }
        public bool EnableCompetition { get; set; }

        public async Task FetchSettings()
        {
            var scope = _serviceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

            var settings = await httpClient.GetFromJsonAsync<SettingsVM>("/api/Settings");

            HustleBenchmarkSeconds = settings.HustleBenchmarkSeconds;
            OutlierSeconds = settings.OutlierSeconds;
            MinDels = settings.MinDels;
            MinTrackedPercentage = settings.MinTrackedPercentage;
            MaxOverspeeds = settings.MaxOverspeeds;
            ShowDeliveries = settings.ShowDeliveries;
            EnableCompetition = settings.EnableCompetition;
        }
    }
}
