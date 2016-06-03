using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using eFocus.VRIS.Web.Models;
using eFocus.VRIS.Web.Models.Data;

namespace eFocus.VRIS.Web.Controllers
{
    public class RoomController : ApiController
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            using (var context = new CalendarContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                return context.Appointments.Select(x => x.Room).Distinct().OrderBy(x => x).ToList();
            }
        }

        [HttpGet]
        public IEnumerable<IRoomAvailability> Get(string id)
        {
            using (var context = new CalendarContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                var appointments = context.Appointments.Where(x => x.Room == id).Include(nameof(Appointment.AppointmentAttendees)).OrderBy(x => x.StartUtc).ToList();
                return CalculateRoomAvailability(appointments);
            }
        }

        public IEnumerable<IRoomAvailability> CalculateRoomAvailability(IEnumerable<Appointment> appointments)
        {
            var enumeratedAppointments = appointments as IList<Appointment> ?? appointments.ToList();
            var roomAvailabilities = new List<IRoomAvailability>(enumeratedAppointments);

            var firstAppointment = enumeratedAppointments.FirstOrDefault();

            if (firstAppointment != null)
            {
                roomAvailabilities.Add(new RoomAvailable
                {
                    StartUtc = DateTime.UtcNow.Date,
                    EndUtc = firstAppointment.StartUtc
                });
            }
            else // No appointments present 
            {
                roomAvailabilities.Add(new RoomAvailable
                {
                    StartUtc = DateTime.UtcNow.Date,
                    EndUtc = DateTime.MaxValue
                });
            }

            for (int i = 0; i < enumeratedAppointments.Count() - 1; i++)
            {
                var appointment = enumeratedAppointments.ElementAt(i);
                var nextAppointment = enumeratedAppointments.ElementAt(i + 1);

                if (nextAppointment.StartUtc - appointment.EndUtc < TimeSpan.FromMinutes(15)) continue;

                roomAvailabilities.Add(new RoomAvailable()
                {
                    StartUtc = appointment.EndUtc,
                    EndUtc =  nextAppointment.StartUtc,
                });
            }

            var lastAppointment = enumeratedAppointments.LastOrDefault();
            if (lastAppointment != null)
            {
                roomAvailabilities.Add(new RoomAvailable
                {
                    StartUtc = lastAppointment.EndUtc,
                    EndUtc = DateTime.MaxValue
                });
            }

            return roomAvailabilities.OrderBy(x => x.StartUtc);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] Appointment appointment)
        {
            try
            {
                using (var context = new CalendarContext())
                {
                    appointment.AppointmentId = Guid.NewGuid().ToString();

                    if (string.IsNullOrWhiteSpace(appointment.Subject))
                        appointment.Subject = "VRIS";

                    appointment.AddOnSync = true;
                    appointment.CreatedByVris = true;

                    context.Appointments.Add(appointment);
                    context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception exception)
            {
                return Content(HttpStatusCode.BadRequest, exception.Message);
            }
        }
    }
}
