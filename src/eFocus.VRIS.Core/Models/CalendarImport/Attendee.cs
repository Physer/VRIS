namespace eFocus.VRIS.Core.Models.CalendarImport
{
    public class Attendee
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsOrganizer { get; set; }
        public bool IsRequired { get; set; }
    }
}