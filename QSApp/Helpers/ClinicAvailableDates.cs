using Microsoft.EntityFrameworkCore;
using QSApp.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QSApp.Helpers
{
    public class ClinicAvailableDates
    {

        public static async Task<IEnumerable<string>> getClinicAvaliableDates(QsDbContext _context, int clinicId, IEnumerable<string> workDays)
        {

            var clinic = await _context.Clinics
            .Where(x => x.Id == clinicId).Include(x => x.Reservations).FirstOrDefaultAsync();

            var reservations =
            clinic
            .Reservations.Where(e => e.ClinicId == clinicId)
            .Where(r => r.DateTime.Date >= DateTime.Today.Date && r.DateTime.Date <= DateTime.Now.AddDays(10).Date).ToList();

            IDictionary<string, int> dict = new Dictionary<string, int>();

            // count how many times the reservation happened in the same date
            // dictionary => {date, number of reservations}
            // Ex dict = { "15/11/2022": 3, "16/11/2022": 1 }
            for (int i = 0; i < reservations.Count(); i++)
            {

                var reservationDate = reservations[i].DateTime.ToShortDateString();
                if (dict.ContainsKey(reservationDate))
                {
                    dict[reservationDate] = ++dict[reservationDate];
                }
                else
                {
                    dict.Add(reservationDate, 1);
                }

            }

            List<string> availableDates = new List<string>();

            // add the available dates in the arr that aren't equal or more than the clinic's limit
            // and check if current date is one of the workdays
            for (int i = 0; i < 20; i++)
            {
                var date = DateTime.Now.AddDays(i);
                var dateShort = date.ToShortDateString();
                if (!(dict.ContainsKey(dateShort) && dict[dateShort] >= clinic.Limit) && workDays.Contains(date.DayOfWeek.ToString()))
                {
                    availableDates.Add(dateShort);
                }
            }
            return availableDates;

        }
    }
}
