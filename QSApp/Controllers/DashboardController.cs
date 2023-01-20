using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSApp.DataContext;
using QSApp.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QSApp.Controllers
{
    public class DashboardController : Controller
    {

        private readonly QsDbContext _context;

        public DashboardController(QsDbContext context)
        {
            _context = context;
        }

        [Route("api/Dashboard-data")]
        [HttpGet]
        public async Task<ActionResult<DashboardDataDTO>> GetDashboardData()
        {
            // total patients
            // total users
            // total clinics
            // total reservations
            var users = await _context.Users.ToListAsync();
            var usersLength = users.Count;

            var patients = await _context.Patients.ToListAsync();
            var patientsLength = patients.Count;

            var clinics = await _context.Clinics.ToListAsync();
            var clinicsLength = clinics.Count;

            var reservations = await _context.Reservations.ToListAsync();
            var reservationsLength = reservations.Count;

            IDictionary<string, int> keyValuePairs = new Dictionary<string, int>
            {
                ["Jan"] = 0,
                ["Feb"] = 0,
                ["Mar"] = 0,
                ["Apr"] = 0,
                ["May"] = 0,
                ["JUN"] = 0,
                ["Jul"] = 0,
                ["Aug"] = 0,
                ["Sept"] = 0,
                ["Oct"] = 0,
                ["Nov"] = 0,
                ["Dec"] = 0,
            };

            for(int i = 0; i < keyValuePairs.Count; i++)
            {
                var item = keyValuePairs.ElementAt(i);
                keyValuePairs[item.Key] = reservations.Where(r => r.DateTime.Month == i + 1).ToList().Count();
            }


            return new DashboardDataDTO()
            {
                Users = usersLength,
                Patients = patientsLength,
                Clinics = clinicsLength,
                Reservations = reservationsLength,
                NumberOfReservationsPerMonth = keyValuePairs
            };
        }


        [Route("api/NumberOfReservationsPerMonthAndYear/{year}")]
        [HttpGet]
        public async Task<IDictionary<string, int>> NumberOfReservationsPerMonthAndYear(int year)
        {

            var reservations = await _context.Reservations.ToListAsync();

            IDictionary<string, int> keyValuePairs = new Dictionary<string, int>
            {
                ["Jan"] = 0,
                ["Feb"] = 0,
                ["Mar"] = 0,
                ["Apr"] = 0,
                ["May"] = 0,
                ["JUN"] = 0,
                ["Jul"] = 0,
                ["Aug"] = 0,
                ["Sept"] = 0,
                ["Oct"] = 0,
                ["Nov"] = 0,
                ["Dec"] = 0,
            };

            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                var item = keyValuePairs.ElementAt(i);
                keyValuePairs[item.Key] = reservations.Where(r => r.DateTime.Month == i + 1 && r.DateTime.Year == year).ToList().Count();
            }
            return keyValuePairs;
        }





        [Route("api/GetReservationsNumberForEachClinic")]
        [HttpGet]
        public async Task<IDictionary<string, int>> GetNumberOfReservationsForEachClinic()
        {
            var clinics = await _context.Clinics.Include(c => c.Reservations).ToListAsync();
            IDictionary<string, int> reservationsCount = new Dictionary<string, int>();
            for (int i = 0; i < clinics.Count; i++)
            {
                var s = clinics[i].Reservations.Count();
                var n = clinics[i].Name;
                reservationsCount.Add(n, s);
            }
            return reservationsCount;
        }
    }
}
