using DominosDriverHustleComp.Models;

namespace DominosDriverHustleComp.Services
{
    public class DeliveryTime
    {
        /// <summary>
        /// DateTime from the GPS
        /// </summary>
        public DateTime EventAt { get; set; }

        /// <summary>
        /// DateTime that the state was changed in our tracker system
        /// </summary>
        public DateTime RecordedAt { get; set; }
    }
    public class DeliveryHTA
    {
        public DeliveryTime DispatchAt { get; set; }
        public DeliveryTime LeftStoreAt { get; set; }
        public DeliveryTime StoreEnterAt { get; set; }
        public DeliveryTime InAt { get; set; }
    }

    public class DriverData
    {
        public DriverStatus DriverStatus { get; set; }
        public DeliveryHTA HTA { get; set; } = new();
    }

    public class DeliveryTrackerService
    {
        private readonly Dictionary<int, DriverData> driverHTAs = [];
        private readonly ILogger<DeliveryTrackerService> _logger;

        public DeliveryTrackerService(ILogger<DeliveryTrackerService> logger)
        {
            _logger = logger;
        }

        public void UpdateDriverStatus(int DriverId, DriverStatus DriverStatus, DateTime DriverStateAt)
        {
            if (!driverHTAs.TryGetValue(DriverId, out DriverData? data))
            {
                data = new()
                {
                    DriverStatus = DriverStatus
                };

                driverHTAs.Add(DriverId, data);
            }
            else if (data.DriverStatus == DriverStatus)
            {
                return;
            }

            switch(DriverStatus)
            {
                case DriverStatus.Dispatched:
                    if (data.HTA.DispatchAt is null)
                        _logger.LogInformation("Set DispatchedAt");

                    data.HTA.DispatchAt ??= new()
                    {
                        EventAt = DriverStateAt,
                        RecordedAt = DateTime.Now
                    };
                    break;

                case DriverStatus.Outbound:
                    if (data.HTA.LeftStoreAt is null)
                        _logger.LogInformation("Set LeftStoreAt");

                    data.HTA.LeftStoreAt ??= new()
                    {
                        EventAt = DriverStateAt,
                        RecordedAt = DateTime.Now
                    };
                    break;

                case DriverStatus.Arrived:
                    if (data.HTA.StoreEnterAt is null)
                        _logger.LogInformation("Set StoreEnteredAt");

                    data.HTA.StoreEnterAt ??= new()
                    {
                        EventAt = DriverStateAt,
                        RecordedAt = DateTime.Now
                    };
                    break;

                case DriverStatus.In:
                    if (data.HTA.InAt is null)
                        _logger.LogInformation("Set InAt");

                    data.HTA.InAt ??= new()
                    {
                        EventAt = DriverStateAt,
                        RecordedAt = DateTime.Now
                    };
                    break;
            }
        }

        public void FinalizeDeliveryForDriver(int DriverId, HeightenedTimeAwareness GPSHTA)
        {
            if (driverHTAs.TryGetValue(DriverId, out var data))
            {
                LogDetailsForDelivery(data.HTA, GPSHTA);
                data.HTA = new();
            }
        }

        private void LogDetailsForDelivery(DeliveryHTA HTA, HeightenedTimeAwareness GPSHTA)
        {
            bool bError = false;

            if (HTA.DispatchAt == null || GPSHTA.DispatchAt == null)
            {
                /*
                bError = true;
                _logger.LogError("DispatchAt is null: {our} | {gps}", HTA.DispatchAt == null, GPSHTA.DispatchAt == null);
                */
            }
            else if (Math.Floor(HTA.DispatchAt.EventAt.TimeOfDay.TotalSeconds) != Math.Floor(GPSHTA.DispatchAt.Value.TimeOfDay.TotalSeconds))
            {
                bError = true;
                _logger.LogError("DispatchAt discrepancy: {our} | {gps}", HTA.DispatchAt.EventAt, GPSHTA.DispatchAt);
            }

            if (HTA.LeftStoreAt == null || GPSHTA.LeftStoreAt == null)
            {
                /*
                bError = true;
                _logger.LogError("LeftStoreAt is null: {our} | {gps}", HTA.LeftStoreAt == null, GPSHTA.LeftStoreAt == null);
                */
            }
            else if (Math.Floor(HTA.LeftStoreAt.EventAt.TimeOfDay.TotalSeconds) != Math.Floor(GPSHTA.LeftStoreAt.Value.TimeOfDay.TotalSeconds))
            {
                bError = true;
                _logger.LogError("LeftStoreAt discrepancy: {our} | {gps}", HTA.LeftStoreAt.EventAt, GPSHTA.LeftStoreAt);
            }

            if (HTA.StoreEnterAt == null || GPSHTA.StoreEnterAt == null)
            {
                /*
                bError = true;
                _logger.LogError("StoreEnterAt is null: {our} | {gps}", HTA.StoreEnterAt == null, GPSHTA.StoreEnterAt == null);
                */
            }
            else if (Math.Floor(HTA.StoreEnterAt.EventAt.TimeOfDay.TotalSeconds) != Math.Floor(GPSHTA.StoreEnterAt.Value.TimeOfDay.TotalSeconds))
            {
                bError = true;
                _logger.LogError("StoreEnterAt discrepancy: {our} | {gps}", HTA.StoreEnterAt.EventAt, GPSHTA.StoreEnterAt);
            }

            if (HTA.InAt == null || GPSHTA.InAt == null)
            {
                /*
                bError = true;
                _logger.LogError("InAt is null: {our} | {gps}", HTA.InAt == null, GPSHTA.InAt == null);
                */
            }
            else if (Math.Floor(HTA.InAt.EventAt.TimeOfDay.TotalSeconds) != Math.Floor(GPSHTA.InAt.Value.TimeOfDay.TotalSeconds))
            {
                bError = true;
                _logger.LogError("InAt discrepancy: {our} | {gps}", HTA.InAt.EventAt, GPSHTA.InAt);
            }

            if (bError)
            {
                _logger.LogInformation("-------------------");
            }

            _logger.LogInformation("Dispatched at: {HTA_DA} | {GPS_DA}\n\tLeftStore at: {HTA_LS} | {GPS_LS}\n\tStoreEnter at: {HTA_SE} | {GPS_SE}\n\tIn at: {HTA_IA} | {GPS_IA}", HTA.DispatchAt?.EventAt, GPSHTA.DispatchAt, HTA.LeftStoreAt?.EventAt, GPSHTA.LeftStoreAt, HTA.StoreEnterAt?.EventAt, GPSHTA.StoreEnterAt, HTA.InAt?.EventAt, GPSHTA.InAt);
        }
    }
}
