namespace DominosDriverHustleComp.Shared.ViewModels
{
    public class ReportRecord
    {
        public string Name { get; set; }
        public float AvgOut { get; set; }
        public float AvgIn { get; set;}
        public int NumDels { get; set; }
        public int NumOverspeeds { get; set; } = 0;
        public float TrackedPercentage { get; set; }
        public int WinStreak { get; set; } = 0;
        public int Outlier { get; set; } = 0;
        public PreviousWeekStats? PreviousWeekStats { get; set; }
        public bool IsDriverDisqualified { get; set; } = false;

        /// <summary>
        /// Only used by the `Report` component.
        /// Do not set this from the server. Theres probably a better
        /// way of handling this but I dont know what it is
        /// </summary>
        public bool IsAverageRecord { get; set; } = false;
    }
}
