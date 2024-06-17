namespace DominosDriverHustleComp.Server.Models.GPS
{
    public class DriverLocation
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Bearing { get; set; }
        public bool StealthModeEnabled { get; set; }
    }
}
