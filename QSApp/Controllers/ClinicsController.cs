using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSApp.DataContext;
using QSApp.DTOs;
using QSApp.Entities;
using QSApp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsController : ControllerBase
    {
        private readonly QsDbContext _context;

        public ClinicsController(QsDbContext context)
        {
            _context = context;
        }

        // GET: api/Clinics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinic()
        {
            //await ReservationStatus.ChangeReservationStatus(_context);
            var data = await _context.Clinics.Include(x => x.Reservations).Select(x => new ClinicDTO()
            {
                Id = x.Id,
                Name = x.Name,
                RangeTo = x.RangeTo,
                Logo = x.Logo,
                RangeFrom = x.RangeFrom,
                Limit = x.Limit,
                ClinicType = x.ClinicType,
                WorkDays = x.ClinicAvaliableDays.Select(ad => ad.WeekDay.Name).ToList(),
                Cost = x.Cost
            }).ToListAsync();

            for (int i = 0; i < data.Count(); i++)
            {
                data[i].AvaliableDates = await ClinicAvailableDates.getClinicAvaliableDates(_context, data[i].Id, data[i].WorkDays);
                data[i].Logo ??= "";
            }

            return Ok(data);
        }

        // GET: api/Clinics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDTO>> GetClinic(int id)
        {
            //await ReservationStatus.ChangeReservationStatus(_context);
            var clinic = await _context
                            .Clinics
                            .Where(x => x.Id == id)
                            .Include(x => x.Reservations)
                            .Select(x => new ClinicDTO()
                            {
                                Id = x.Id,
                                Name = x.Name,
                                RangeTo = x.RangeTo,
                                Logo = x.Logo,
                                Limit = x.Limit,
                                RangeFrom = x.RangeFrom,
                                WorkDays = x.ClinicAvaliableDays.Select(ad => ad.WeekDay.Name).ToList(),
                                ClinicType = x.ClinicType,
                                Cost = x.Cost,
                                Reservations = x.Reservations.Select(r => new ReservationDTO()
                                {
                                    Id = r.Id,
                                    UserId = r.UserId,
                                    NationalId = r.Patient.User.NationalId,
                                    PatientName = r.Patient.User.UserName,
                                    ClinicId = r.ClinicId,
                                    PatientId = r.PatientId,
                                    DateTime = r.DateTime.ToShortDateString(),
                                    Cost = x.Cost,
                                    PatientNumber = r.PatientNumber,
                                    Status = r.Status.Name,
                                    StatusId = r.Status.Id,
                                    PhoneNumber = r.Patient.User.PhoneNumber,
                                    ConfirmedDate = r.ConfirmedDate.ToShortDateString(),
                                    QRCode = r.QRCode
                                }).ToList()
                            })
                            .FirstOrDefaultAsync();

            if (clinic == null) return BadRequest("Invalid ClinicId");
            clinic.AvaliableDates = await ClinicAvailableDates.getClinicAvaliableDates(_context, clinic.Id, clinic.WorkDays);


            if (clinic == null)
            {
                return NotFound();
            }

            return clinic;
        }
        // GET: api/Clinics/Get-today-tomorrow-Reservations/5
        [Route("Get-today-tomorrow-Reservations/{id}")]
        [HttpGet]
        public async Task<ActionResult<TodayTomorrowResDTO>> GetClinicReservations(int id)
        {
            //await ReservationStatus.ChangeReservationStatus(_context);
            var today = DateTime.Today.Date;
            var tomorrow = today.AddDays(1).Date;

            var clinic = await _context
                            .Clinics
                            .Where(x => x.Id == id)
                            .Include(x => x.Reservations)
                                .ThenInclude(x => x.Status)
                            .Include(x => x.Reservations)
                                .ThenInclude(x => x.Patient)
                                    .ThenInclude(x => x.User)
                            .FirstOrDefaultAsync();


            if (clinic == null) return NotFound("Invalid ClinicId");

            var todayR = clinic.Reservations.Where(r => r.DateTime.Date == today
            && !IsColumn(r.StatusId, 6)).Select(r => new ReservationDTO()
            {
                Id = r.Id,
                PatientId = r.PatientId,
                DateTime = r.DateTime.ToShortDateString(),
                ClinicId = r.ClinicId,
                Status = r.Status.Name,
                StatusId = r.StatusId,
                UserId = r.UserId,
                PatientName = r.Patient.User.UserName,
                ClinicName = r.Clinic.Name,
                Cost = r.Clinic.Cost,
                PatientNumber = r.PatientNumber,
                PhoneNumber = r.Patient.User.PhoneNumber
            }).OrderBy(r => r.PatientNumber).ToList();


            var tomorrowR = clinic.Reservations.Where(r => r.DateTime.Date == tomorrow
            && !IsColumn(r.StatusId, 6, 2)).Select(r => new ReservationDTO()
            {
                Id = r.Id,
                PatientId = r.PatientId,
                DateTime = r.DateTime.ToShortDateString(),
                ClinicId = r.ClinicId,
                PatientName = r.Patient.User.UserName,
                ClinicName = r.Clinic.Name,
                Status = r.Status.Name,
                StatusId = r.StatusId,
                UserId = r.UserId,
                Cost = r.Clinic.Cost,
                PatientNumber = r.PatientNumber,
                PhoneNumber = r.Patient.User.PhoneNumber
            }).OrderBy(r => r.PatientNumber).ToList();

            return new TodayTomorrowResDTO()
            {
                Today = todayR,
                Tomorrow = tomorrowR
            };
        }

        private bool IsColumn(int col, params int[] numbers) => numbers.Any(n => n == col);



        // get the number of reservations per month for a clinic
        [Route("Reservations/{clinicId}/{date}")]
        [HttpGet]
        public async Task<int> NoOfReservationsPerMonth(int clinicId, string date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            var clinic = await _context.Clinics.Include(c => c.Reservations)
                .Where(c => c.Id == clinicId).FirstOrDefaultAsync();

            var t = clinic.Reservations.Where(r => r.DateTime.ToShortDateString() == dateTime.ToShortDateString()).ToList();
            return t.Count;
        }



        // PUT: api/Clinics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinic(int id, [FromForm] ClinicDTO clinic)
        {
            if (id != clinic.Id)
                return BadRequest();

            var clinicToEdit = await _context
                            .Clinics
                            .Where(x => x.Id == id)
                            .Include(x => x.Reservations)
                            .Include(x => x.ClinicAvaliableDays)
                                .ThenInclude(x => x.WeekDay)
                            .FirstOrDefaultAsync();


            double clinicCost = 0;
            switch (clinic.ClinicType)
            {
                case "specialist":
                    clinicCost = 100;
                    break;
                case "advisor":
                    clinicCost = 200;
                    break;
            }

            clinicToEdit.Name = clinic.Name;
            clinicToEdit.RangeFrom = clinic.RangeFrom;
            clinicToEdit.RangeTo = clinic.RangeTo;
            clinicToEdit.Limit = clinic.Limit;
            clinicToEdit.ClinicType = clinic.ClinicType;
            clinicToEdit.Cost = clinicCost;


            if (clinic.WorkDays?.Any() == true)
            {
                var updateWorkDays = clinic.WorkDays.ToList();

                var updateWeekdaysIds = await _context.WeekDays
                    .Where(weekday => updateWorkDays.Contains(weekday.Name))
                    .Select(weekday => weekday.Id).ToListAsync();
                var updateClinicAvailableDays =
                    updateWeekdaysIds.Select(weekdayId => new ClinicAvailableDays()
                    {
                        WeekDayId = weekdayId,
                        Clinic = clinicToEdit,
                    }).ToList();

                var removeCurrentAvailableDays = await _context.ClinicAvaliableDays.Where(c => c.ClinicId == clinic.Id).ToListAsync();


                _context.ClinicAvaliableDays.RemoveRange(removeCurrentAvailableDays);
                _context.ClinicAvaliableDays.AddRange(updateClinicAvailableDays);
            }



            if (clinic.LogoFile?.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    clinic.LogoFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    clinicToEdit.Logo = Convert.ToBase64String(fileBytes);
                }
            }


            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Clinics
        [HttpPost]
        public async Task<ActionResult<Clinic>> PostClinic(ClinicDTORequestBody clinic)
        {

            double clinicCost = 0;
            switch (clinic.ClinicType)
            {
                case "specialist":
                    clinicCost = 100;
                    break;
                case "advisor":
                    clinicCost = 200;
                    break;
            }

            // created the clinic object
            var clinictToAdd = new Clinic()
            {
                Name = clinic.Name,
                RangeFrom = clinic.RangeFrom,
                RangeTo = clinic.RangeTo,
                Limit = clinic.Limit,
                Latitude = clinic.Latitude,
                Longitude = clinic.Longitude,
                ClinicType = clinic.ClinicType,
                Cost = clinicCost
            };



            // got the workdays array from the user and put it in a variable
            // EX. workDays = ["Monday", "Tuesday", "Friday"]
            var workDays = clinic.WorkDays;
            // got the weekdaysIds from the database table "WeekDays"
            // Ex weekdaysIds = [3, 4, 7] => Monday's Id is 3, Tuesday's Id is 4 and Friday's Id is 7
            var weekdaysIds = await _context.WeekDays.Where(weekday => workDays.Contains(weekday.Name)).Select(weekday => weekday.Id).ToListAsync();


            if ((clinic.LogoFile != null) && (clinic.LogoFile.Length > 0))
            {
                using (var ms = new MemoryStream())
                {
                    clinic.LogoFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    clinictToAdd.Logo = Convert.ToBase64String(fileBytes);
                }
            }
            /*else
            {
                string path = "Images/empty-image.png";
                var fileBytes = System.IO.File.ReadAllBytes(path);
                clinictToAdd.Logo = Convert.ToBase64String(fileBytes);
            }*/

            // create a array of ClinicAvaliableDay to pass it to the database table called "ClinicAvaliableDay"
            // Ex clinicAvaliableDays = [{WeekDayId: 3, ClinicId: 1019},{WeekDayId: 4, ClinicId: 1019}], {WeekDayId: 7, ClinicId: 1019}
            // NOTE: made doctor foreign key as not unqiue to evade adding him in the object currently
            var clinicAvaliableDays =
                weekdaysIds.Select(weekdayId => new ClinicAvailableDays()
                {
                    WeekDayId = weekdayId,
                    Clinic = clinictToAdd,
                }).ToList();

            // we will add here the ClinicAvaliableDays array of objects to the database
            // NOTE: we didn't need to add Clinic Object to the database because we added the entire object
            // inside the ClinicAvaliableDay Obj, EF is so smart that it automatically adds the clinic Object and links it to ClinicAvaliableDay table/Obj
            _context.ClinicAvaliableDays.AddRange(clinicAvaliableDays);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClinic", clinic);
        }


        // DELETE: api/Clinics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ClinicExists(int id)
        {
            return _context.Clinics.Any(e => e.Id == id);
        }







    }
}
