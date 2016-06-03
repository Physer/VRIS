using System.ComponentModel.DataAnnotations;
using System.Linq;
using eFocus.VRIS.Web.Services;
using Newtonsoft.Json;

namespace eFocus.VRIS.Web.Models.Data
{
    [MetadataType(typeof(AppointmentMetadata))]
    public partial class Appointment : IRoomAvailability
    {
        public bool Available => false;
        public AppointmentAttendee Organizer => AppointmentAttendees.FirstOrDefault(x => x.IsOrganizer);
        public Core.Models.Branding.Organization OrganizationBranding => BrandingService.GetOrganizationForEmails(AppointmentAttendees.Select(x => x.Email));
    }

    public class AppointmentMetadata
    {

    }

    
}
