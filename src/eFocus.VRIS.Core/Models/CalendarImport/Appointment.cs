using System;
using System.Collections.Generic;

namespace eFocus.VRIS.Core.Models.CalendarImport
{
    public class Appointment
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool AllDay { get; set; }
        public IEnumerable<Attendee> Attendees { get; set; }
    }
}