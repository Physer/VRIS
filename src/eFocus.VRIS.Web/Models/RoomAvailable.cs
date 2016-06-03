using System;

namespace eFocus.VRIS.Web.Models
{
    public class RoomAvailable : IRoomAvailability
    {
        public bool Available => true;
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
    }
}
