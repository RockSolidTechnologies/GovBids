using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GovBids.Entities
{
    /// <summary>
    /// Devices registered for push notification
    /// </summary>
    public class Device
    {
        public Device() { }

        /// <summary>
        /// Id of the device table
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// Device unique idendentifier to communicate with different services
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        /// Type of the device
        /// <remarks>Windows Phone; iOS</remarks>
        /// </summary>
        public string DeviceType { get; set; }
    }
}