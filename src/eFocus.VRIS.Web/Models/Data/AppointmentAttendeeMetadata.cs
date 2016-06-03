using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace eFocus.VRIS.Web.Models.Data
{
    [MetadataType(typeof(AppointmentAttendeeMetadata))]
    public partial class AppointmentAttendee
    {
        public string FirstName => Name.Split(' ')[0].Trim();
        public string LastName => Name.Replace(FirstName, "").Trim();
    }

    public class AppointmentAttendeeMetadata
    {
        [JsonIgnore]
        public int AppointmentAttendeeId { get; set; }
        [JsonIgnore]
        public string AppointmentId { get; set; }
    }

    
}
