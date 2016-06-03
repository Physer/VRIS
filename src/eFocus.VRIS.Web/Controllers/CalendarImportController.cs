using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using eFocus.VRIS.Core.Models.CalendarImport;
using eFocus.VRIS.Web.Models.Data;

namespace eFocus.VRIS.Web.Controllers
{
    public class CalendarImportController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]IEnumerable<Room> rooms)
        {
            using (var context = new CalendarContext())
            {
                // Remove missing appointments from database
                var appointmentIdsToDelete = context.Appointments.Select(x => x.AppointmentId).Except(rooms.SelectMany(x => x.Appointments.Select(y => y.Id)));
                foreach (var id in appointmentIdsToDelete)
                    context.Appointments.Remove(context.Appointments.Find(id));

                foreach (var room in rooms)
                {
                    foreach (var appointment in room.Appointments)
                    {
                        var existingAppointment = context.Appointments.Find(appointment.Id);

                        if (existingAppointment != null) // Update appointment
                        {
                            existingAppointment.Subject = appointment.Subject;
                            existingAppointment.Room = room.Name;
                            existingAppointment.StartUtc = appointment.StartUtc;
                            existingAppointment.EndUtc = appointment.EndUtc;
                            existingAppointment.IsAllDay = appointment.AllDay;

                            // Remove missing attendees from database
                            var appointmentAttendeesEmailsToDelete = existingAppointment.AppointmentAttendees.Select(x => x.Email).Except(appointment.Attendees.Select(x => x.Email));
                            foreach (var email in appointmentAttendeesEmailsToDelete)
                                existingAppointment.AppointmentAttendees.Remove(existingAppointment.AppointmentAttendees.FirstOrDefault(x => x.Email == email));

                            foreach (var attendee in appointment.Attendees)
                            {
                                var existingAttendee = existingAppointment.AppointmentAttendees.FirstOrDefault(x => x.Email == attendee.Email);

                                if (existingAttendee != null) // Update attendee
                                {
                                    existingAttendee.Name = attendee.Name;
                                    existingAttendee.IsOrganizer = attendee.IsOrganizer;
                                    existingAttendee.IsRequired = attendee.IsRequired;
                                }
                                else // Add new attendee
                                {
                                    var newAttendee = context.AppointmentAttendees.Create();

                                    newAttendee.Email = attendee.Email;
                                    newAttendee.Name = attendee.Name;
                                    newAttendee.IsOrganizer = attendee.IsOrganizer;
                                    newAttendee.IsRequired = attendee.IsRequired;

                                    existingAppointment.AppointmentAttendees.Add(newAttendee);
                                }
                            }
                        }
                        else // Add new appointment
                        {
                            var newAppointment = context.Appointments.Create();

                            newAppointment.AppointmentId = appointment.Id;
                            newAppointment.Subject = appointment.Subject;
                            newAppointment.Room = room.Name;
                            newAppointment.StartUtc = appointment.StartUtc;
                            newAppointment.EndUtc = appointment.EndUtc;
                            newAppointment.IsAllDay = appointment.AllDay;

                            foreach (var attendee in appointment.Attendees)
                            {
                                newAppointment.AppointmentAttendees.Add(new AppointmentAttendee
                                {
                                    Email = attendee.Email,
                                    Name = attendee.Name,
                                    IsOrganizer = attendee.IsOrganizer,
                                    IsRequired = attendee.IsRequired
                                });
                            }

                            context.Appointments.Add(newAppointment);
                        }
                    }
                }

                await context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
