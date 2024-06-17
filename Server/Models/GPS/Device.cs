namespace DominosDriverHustleComp.Server.Models.GPS
{
    public class Device
    {
        public string DeviceLocationPermissions { get; set; }
        public string DeviceLocationSettings { get; set; }
        public int BatteryLevel { get; set; }
        public bool BatteryOptimizationOn { get; set; }
    }
}
