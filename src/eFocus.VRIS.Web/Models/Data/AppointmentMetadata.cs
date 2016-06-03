using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace eFocus.VRIS.Web.Models.Data
{
    [MetadataType(typeof(AppointmentMetadata))]
    public partial class Appointment : IRoomAvailability
    {
        public bool Available => false;
    }

    public class AppointmentMetadata
    {
        [JsonIgnore]
        public string AppointmentId { get; set; }
    }

    
}
