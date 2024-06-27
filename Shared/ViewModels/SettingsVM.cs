namespace DominosDriverHustleComp.Shared.ViewModels
{
    public class SettingsVM
    {
        public int HustleBenchmarkSeconds { get; set; }
        public int OutlierSeconds { get; set; }
        public int MinDels { get; set; }
        public int MinTrackedPercentage { get; set; }
        public int MaxOverspeeds { get; set; }
        public bool ShowDeliveries { get; set; }
    }
}
