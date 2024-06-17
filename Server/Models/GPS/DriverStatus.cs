namespace DominosDriverHustleComp.Server.Models.GPS
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
}
