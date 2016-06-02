using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using eFocus.VRIS.Core.Models.CalendarImport;
using Microsoft.Office.Interop.Outlook;
using Newtonsoft.Json;

// http://stackoverflow.com/questions/90899/net-get-all-outlook-calendar-items

namespace eFocus.VRIS.CalendarPushService
{
    class Program
    {
        static void Main(string[] args)
        {
            Application oApp = new Application();
            _mapiNamespace = oApp.GetNamespace("MAPI");

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await PostAllCalendarItems();

                    var delay = TimeSpan.FromMinutes(5);
                    
                    Console.WriteLine();
                    Console.WriteLine($"Sleeping for {delay} until next sync");
                    await Task.Delay(delay);
                }
            }, TaskCreationOptions.LongRunning);
               
            Console.ReadLine();
        }

        private static NameSpace _mapiNamespace;

        private static async Task PostAllCalendarItems()
        {
            var rooms = new List<Room>();

            Console.WriteLine("Fetching appointments for AMS 5A");
            var roomAUser = GetUserByName("AMS 5A");
            Recipient roomARecipient = _mapiNamespace.GetRecipientFromID(roomAUser.ID);
            MAPIFolder roomACalendar = _mapiNamespace.GetSharedDefaultFolder(roomARecipient, OlDefaultFolders.olFolderCalendar);
            rooms.Add(new Room
            {
                Name = "AMS 5A",
                Appointments = new List<Appointment>(GetAppointmentsForFolder(roomACalendar)),
            });

            Marshal.ReleaseComObject(roomARecipient);
            Marshal.ReleaseComObject(roomACalendar);

            Console.WriteLine();
            Console.WriteLine("Fetching appointments for AMS 5B");
            var roomBUser = GetUserByName("AMS 5B");
            Recipient roomBRecipient = _mapiNamespace.GetRecipientFromID(roomBUser.ID);
            MAPIFolder roomBCalendar = _mapiNamespace.GetSharedDefaultFolder(roomBRecipient, OlDefaultFolders.olFolderCalendar);
            rooms.Add(new Room
            {
                Name = "AMS 5B",
                Appointments = new List<Appointment>(GetAppointmentsForFolder(roomBCalendar)),
            });

            Marshal.ReleaseComObject(roomBRecipient);
            Marshal.ReleaseComObject(roomBCalendar);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:10510/");
                var content = new StringContent(JsonConvert.SerializeObject(rooms), Encoding.UTF8, "application/json");
                await client.PostAsync("/api/calendarimport", content);
            }

            Console.WriteLine();
            Console.WriteLine("Done! Hit enter to exit.");
        }

        private static IEnumerable<Appointment> GetAppointmentsForFolder(MAPIFolder folder)
        {
            Items outlookCalendarItems = folder.Items;
            outlookCalendarItems.IncludeRecurrences = true;
            var filter = $"[Start] >= '{DateTime.UtcNow.AddDays(-1).ToString("g")}'";
            outlookCalendarItems = outlookCalendarItems.Restrict(filter);

            var appointments = new List<Appointment>();

            foreach (AppointmentItem item in outlookCalendarItems)
            {
                Appointment appointment = null;

                if (item.IsRecurring)
                {
                    RecurrencePattern rp = item.GetRecurrencePattern();
                    var yesterday = DateTime.Today.AddDays(-1);
                    DateTime first = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, item.Start.Hour,
                        item.Start.Minute, 0);

                    DateTime last = DateTime.Today.AddMonths(1);

                    AppointmentItem recur = null;

                    for (DateTime cur = first; cur <= last; cur = cur.AddDays(1))
                    {
                        try
                        {
                            recur = rp.GetOccurrence(cur);
                            appointments.Add(BuildAppointment(recur, cur - first));
                            Console.WriteLine(recur.Subject + " -> " + cur.ToLongDateString());
                        }
                        catch
                        {
                        }
                    }

                    if (recur != null)
                        Marshal.ReleaseComObject(recur);
                }
                else
                {
                    appointments.Add(BuildAppointment(item, TimeSpan.Zero));
                    Console.WriteLine(item.Subject + " -> " + item.Start.ToLongDateString());
                }

                Marshal.ReleaseComObject(item);
                GC.Collect();
            }

            return appointments;
        }

        private static Appointment BuildAppointment(AppointmentItem item, TimeSpan recurrenceOffset)
        {
            var organizer = item.GetOrganizer();

            var attendees = new List<Attendee>
            {
                new Attendee()
                {
                    Name = organizer.Name,
                    Email = GetEmailAddress(organizer.PropertyAccessor),
                    IsOrganizer = true,
                    IsRequired = true,
                }
            };

            Marshal.ReleaseComObject(organizer);

            foreach (Recipient recipient in item.Recipients)
            {
                var attendee = new Attendee()
                {
                    Name = recipient.Name,
                    Email = GetEmailAddress(recipient.PropertyAccessor),
                    IsOrganizer = false,
                    IsRequired = IsRecipientRequired(recipient),
                };

                if (!attendees.Any(x => x.Email.Equals(attendee.Email, StringComparison.InvariantCultureIgnoreCase)))
                    attendees.Add(attendee);

                Marshal.ReleaseComObject(recipient);
            }

            var appointment = new Appointment()
            {
                Subject = item.Subject,
                StartUtc = item.StartUTC + recurrenceOffset,
                EndUtc = item.EndUTC + recurrenceOffset,
                AllDay = item.AllDayEvent,
                Attendees = attendees
            };

            return appointment;
        }

        private static string GetEmailAddress(PropertyAccessor propertyAccessor)
        {
            string PROPERTY_TAG_SMTP_ADDRESS = @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";

            return propertyAccessor.GetProperty(PROPERTY_TAG_SMTP_ADDRESS);
        }

        private static bool IsRecipientRequired(Recipient recipient)
        {
            return recipient.Type == (int)OlMailRecipientType.olTo;
        }

        private static ExchangeUser GetUserByName(string name)
        {
            foreach (AddressEntry address in _mapiNamespace.GetGlobalAddressList().AddressEntries)
            {
                if (address.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return address.GetExchangeUser();
            }

            return null;
        }
    }
}
