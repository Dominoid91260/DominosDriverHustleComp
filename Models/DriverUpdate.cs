using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DominosDriverHustleComp.Models
{
    public enum DriverStatus
    {
        /// <summary>
        /// Driver is signed out on a delivery but is still in the store geofence
        /// </summary>
        Dispatched,

        /// <summary>
        /// Driver has left the store geofence
        /// </summary>
        Outbound,

        /// <summary>
        /// Driver is within the customer geofence
        /// </summary>
        Delivering,

        /// <summary>
        /// Driver is coming back to the store
        /// </summary>
        Inbound,

        /// <summary>
        /// Driver has entered the store geofence
        /// </summary>
        Arrived,

        /// <summary>
        /// Driver has signed back in
        /// </summary>
        In
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
    public class DriverUpdate
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required int DriverId { get; set; }

        public DateTime DriverStatusAt { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DriverStatus DriverStatus { get; set; }

        public required HeightenedTimeAwareness HeightenedTimeAwareness { get; set; }
    }
}
