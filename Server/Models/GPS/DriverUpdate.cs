using System.Text.Json.Serialization;

namespace DominosDriverHustleComp.Server.Models.GPS
{
    public class DriverUpdate
    {
        // assignmentAvailability

        public string FirstName { get; set; }

        // storeNumber

        public string LastName { get; set; }

        // nickname

        public DriverLocation Location { get; set; }
        public int DriverId { get; set; }

        // employeeCode
        // orders
        // deviceId
        // returnToStoreEta
        // emergencyType

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DriverStatus DriverStatus { get; set; }

        public DateTime DriverStatusAt { get; set; }

        public HeightenedTimeAwareness HeightenedTimeAwareness { get; set; }

        // vehicleType

        public Device Device { get; set; }

        // positionCode
        // availableForDispatch
        // sequence
    }
}
