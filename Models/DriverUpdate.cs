using System.Text.Json.Serialization;

namespace DominosDriverHustleComp.Models
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
    public class DriverUpdate
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required int DriverId { get; set; }

        public required HeightenedTimeAwareness HeightenedTimeAwareness { get; set; }
    }
}
