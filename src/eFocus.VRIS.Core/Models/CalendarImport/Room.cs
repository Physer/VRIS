using System.Collections.Generic;

namespace eFocus.VRIS.Core.Models.CalendarImport
{
    public class Room
    {
        public string Name { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }
    }
}