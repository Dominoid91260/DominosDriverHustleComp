namespace DominosDriverHustleComp.Shared.ViewModels
{
    public class SettingsVM
    {
        public float HustleBenchmarkSeconds { get; set; }
        public float OutlierSeconds { get; set; }
        public int MinDels { get; set; }
        public float MinTrackedPercentage { get; set; }
        public int MaxOverspeeds { get; set; }
    }
}
