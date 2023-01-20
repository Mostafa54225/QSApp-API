using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSApp.DataContext;
using QSApp.DTOs;
using QSApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly QsDbContext _context;

        public ReservationsController(QsDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservation()

                                              => await _context
                                                      .Reservations
                                                      .Include(x => x.Clinic)
                                                      .Include(x => x.Patient)
                                                      .ThenInclude(x => x.User)
                                                      .Select(x => new ReservationDTO()
                                                      {
                                                          ClinicId = x.ClinicId,
                                                          Cost = x.Clinic.Cost,
                                                          DateTime = x.DateTime.Date.ToShortDateString(),
                                                          Id = x.Id,
                                                          PatientId = x.PatientId,
                                                          UserId = x.UserId,
                                                          PatientNumber = x.PatientNumber,
                                                          QRCode = x.QRCode,
                                                          StatusId = x.StatusId,
                                                          PatientName = x.Patient.User.UserName,
                                                          ClinicName = x.Clinic.Name,
                                                          //Clinic = x.Clinic,
                                                          Status = x.Status.Name,
                                                          //Patient = x.Patient
                                                          PhoneNumber = x.Patient.User.PhoneNumber,
                                                          ConfirmedDate = x.ConfirmedDate.ToShortDateString()
                                                      }).ToListAsync();

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var reservation = await _context.Reservations.Where(x => x.Id == id)
                .Include(c => c.Clinic)
                .Include(x => x.Patient)
                    .ThenInclude(x => x.User)
                    .Select(x => new ReservationDTO()
                    {
                        ClinicId = x.ClinicId,
                        Cost = x.Clinic.Cost,
                        DateTime = x.DateTime.Date.ToShortDateString(),
                        Id = x.Id,
                        PatientId = x.PatientId,
                        UserId = x.UserId,
                        PatientNumber = x.PatientNumber,
                        QRCode = x.QRCode,
                        StatusId = x.StatusId,
                        PatientName = x.Patient.User.UserName,
                        ClinicName = x.Clinic.Name,
                        //Clinic = x.Clinic,
                        Status = x.Status.Name,
                        //Patient = x.Patient
                        PhoneNumber = x.Patient.User.PhoneNumber,
                        ConfirmedDate = x.ConfirmedDate.ToShortDateString()
                    }).FirstOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        [Route("user/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationUserDTO>>> GetUserReservation(int id)
        {
            var today = DateTime.Today.Date;
            List<ReservationUserDTO> arr = new();
            var reservations = await _context.Reservations.Where(r => r.UserId == id)
                .Include(x => x.Clinic)
                .Include(x => x.Patient)
                    .ThenInclude(x => x.User)
                .OrderByDescending(x => x.DateTime)
                .Select(x => new ReservationUserDTO()
                {
                    Id = x.Id,
                    Cost = x.Clinic.Cost,
                    DateTime = x.DateTime.ToShortDateString(),
                    PatientNumber = x.PatientNumber,
                    PatientName = x.Patient.User.UserName,
                    ClinicName = x.Clinic.Name,
                    ConfirmedDate = x.ConfirmedDate.ToShortDateString(),
                    Status = x.Status.Name,
                    PhoneNumber = x.Patient.User.PhoneNumber,
                    ClinicId = x.Clinic.Id,
                })
                .ToListAsync();

            var todayReservations = reservations.Where(r => Convert.ToDateTime(r.DateTime) == today).ToList();
            var commingReservations = reservations.Where(r => Convert.ToDateTime(r.DateTime) > today)
                .OrderBy(r => r.DateTime).ToList();
            var pastReservations = reservations.Where(r => Convert.ToDateTime(r.DateTime) < today).ToList();
            arr.AddRange(todayReservations);
            arr.AddRange(commingReservations);
            arr.AddRange(pastReservations);

            return arr;
        }





        // GET: api/Reservations/by-date/2022-11-20

        [Route("by-date/{date}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservation(DateTime date)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Status).Include(r => r.Clinic).ToListAsync();


            var t = reservations.Where(r =>
            r.DateTime.ToShortDateString() == date.ToShortDateString()).Select(r => new ReservationDTO()
            {
                Id = r.Id,
                PatientId = r.PatientId,
                DateTime = r.DateTime.ToShortDateString(),
                ClinicId = r.ClinicId,
                Status = r.Status.Name,
                StatusId = r.StatusId,
                UserId = r.UserId,
                PatientNumber = r.PatientNumber,
                Cost = r.Clinic.Cost,
                ClinicName = r.Clinic.Name,
                ConfirmedDate = r.ConfirmedDate.ToShortDateString(),

            }).ToList();

            if (t.Count == 0) return NotFound("There is no reservations in this day");

            return t;

        }


        // PUT: api/Reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: api/Reservations
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(ReservationDTORequestBody reservation)
        {

            DateTime myDate = default(DateTime);
            var now = DateTime.Now;
            if (reservation.DateTime == myDate) reservation.DateTime = now.Date;
            Patient patient = await _context.Patients.Where(p => p.UserId == reservation.UserId).FirstOrDefaultAsync();
            if (patient == null) return BadRequest("only patients can reserve");
            int patientId = patient.Id;
            //reservation.PatientId = patientId;
            Clinic clinic = await _context.Clinics
                .Include(c => c.Reservations)
                .Include(c => c.ClinicAvaliableDays)
                .ThenInclude(c => c.WeekDay)
                .Where(c => c.Id == reservation.ClinicId).FirstOrDefaultAsync();

            var isReservedBefore = clinic.Reservations.Where(r => r.DateTime.ToShortDateString() ==
            reservation.DateTime.ToShortDateString() && r.UserId == reservation.UserId).FirstOrDefault();
            if (isReservedBefore != null) return BadRequest("You can't reserve more than one reservation in the same day");

            if (clinic == null) return BadRequest("This clinic doesn't exist");

            //if (reservation.StatusId == 0) reservation.StatusId = 1;


            int userId = reservation.UserId;

            var reservations = await _context.Reservations.Where(r => r.UserId == userId).ToListAsync();


            IEnumerable<string> workDays = clinic.ClinicAvaliableDays.Select(ad => ad.WeekDay.Name).ToList();

            // check if the date of reservation is one of the work days of the clinic
            var dayOfWeek = reservation.DateTime.DayOfWeek.ToString();
            if (!workDays.Contains(dayOfWeek))
            {
                return BadRequest("This clinic doesn't work today");
            }

            // count how many times the reservation happened in the same date
            int reservationsCount = clinic.Reservations
                .Where(r => r.DateTime.ToShortDateString() == reservation.DateTime.ToShortDateString()).Count();


            if (reservationsCount >= clinic.Limit)
            {
                return BadRequest("Exceeded clinic Reservation Limit for this day.");
            }


            _context.Reservations.Add(new()
            {
                UserId = reservation.UserId,
                PatientId = patientId,
                StatusId = 1,
                //Cost = reservation.Cost,
                DateTime = reservation.DateTime,
                ClinicId = reservation.ClinicId,
                PatientNumber = 0,
                //QRCode = reservation.QRCode
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservation", reservation);
        }
        private bool IsColumn(int col, params int[] numbers) => numbers.Any(n => n == col);


        [Route("change-reservation-status/{reservationId}/{status}")]
        [HttpPut]
        // status is 1 or 0 | 1 = confirmed, 0 = canceled
        public async Task<IActionResult> ChangeReservationStatus(int reservationId, int status)
        {
            if (status != 0 && status != 1)
            {
                return BadRequest("you should write 1 or 0 in status" +
                    "\n1 to confirm || 0 to not confirm");
            }
            var today = DateTime.Today.Date;

            var reservation = await _context.Reservations.Where(r => r.Id == reservationId).FirstOrDefaultAsync();

            if (reservation == null) return BadRequest("No reservation exist with this id");

            if (reservation == null) return BadRequest("There is no reservation with this id");

            if (reservation.StatusId == 2 && status == 1) return NoContent();
            if (IsColumn(reservation.StatusId, 4, 5, 6)) return NoContent();
            //if (reservation.StatusId == 2) return NoContent();
            var clinic = await _context.Clinics.Where(c => c.Id == reservation.ClinicId)
                .Include(c => c.Reservations).FirstOrDefaultAsync();
            if (status == 1)
            {
                var lastReservationNumber = 1;
                var ConfirmedReservation = clinic.Reservations
                    .Where(r => r.DateTime.ToShortDateString() == reservation.DateTime.ToShortDateString())
                    .Where(r => r.StatusId != 1 && r.StatusId != 6)
                    //.Where(r => r.PatientNumber)
                    //.Where(r => r.PatientNumber == clinic.Reservations.Max(r2 => (int?)r2.PatientNumber))
                    .ToList();

                var lastConfirmedReservation = ConfirmedReservation.Where(
                        r => !ConfirmedReservation.Any(e => e.PatientNumber > r.PatientNumber)).FirstOrDefault();


                if (lastConfirmedReservation != null)
                {
                    lastReservationNumber = lastConfirmedReservation.PatientNumber + 1;
                }
                reservation.PatientNumber = lastReservationNumber;
                reservation.StatusId = 2;
                reservation.ConfirmedDate = DateTime.Now;

            }
            else if (status == 0)
            {
                reservation.StatusId = 6;
                reservation.PatientNumber = 0;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Route("next-reservation/{clinicId}")]
        [HttpPut]
        public async Task<IActionResult> NextReservation(int clinicId)
        {
            var today = DateTime.Today.Date;
            var clinic = await _context
                            .Clinics
                            .Where(x => x.Id == clinicId)
                            .Include(x => x.Reservations).FirstOrDefaultAsync();
            var todayReservations = clinic.Reservations
                .Where(r => r.DateTime.Date == today)
                .Where(r => r.StatusId != 1 && r.StatusId != 3 && r.StatusId != 6)
                .OrderBy(r => r.ConfirmedDate).ThenBy(r => r.PatientNumber).ToList();

            int numberOfPatientsReserved = 0;
            //int numberOfCurrentPatient = 0;
            for (int i = 0; i < todayReservations.Count(); i++)
            {

                if (todayReservations[i].StatusId == 4)
                {
                    todayReservations[i].StatusId = 5;
                    if (i == todayReservations.Count - 1)
                        break;

                    todayReservations[i + 1].StatusId = 4;
                    //numberOfCurrentPatient = todayReservations[i + 1].PatientNumber;
                    break;
                }
                else if (todayReservations[i].StatusId == 5)
                {
                    ++numberOfPatientsReserved;
                    continue;
                }
                else if (todayReservations[i].StatusId == 2)
                {
                    todayReservations[i].StatusId = 4;
                    //if(todayReservations.Count)
                    //numberOfCurrentPatient = todayReservations[i + 1].PatientNumber;
                    break;
                }

            }
            if (numberOfPatientsReserved == todayReservations.Count())
            {
                return Content("All Confirmed Patients are reserved today.");
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation is null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Route("{clinicId}/{date}")]
        [HttpGet]
        public async Task<int> GetNoReservationsOfClinicByDate(int clinicId, string date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            var clinic = await _context.Clinics.Where(c => c.Id == clinicId).Include(c => c.Reservations).FirstOrDefaultAsync();
            return clinic.Reservations
                .Where(r => r.DateTime.ToShortDateString() == dateTime.ToShortDateString())
                .Count();
        }



        [Route("{clinicId}/{userId}/{date}")]
        [HttpGet]
        public async Task<ActionResult<GetDataOfReservationsDTO>> GetDataOfReservations(int clinicId, int userId, string date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            var clinic = await _context.Clinics.Where(c => c.Id == clinicId)
                .Include(c => c.Reservations).FirstOrDefaultAsync();
            var reservationsByDate = clinic.Reservations
                .Where(r => r.DateTime.ToShortDateString() == dateTime.ToShortDateString()
                && r.StatusId != 6)
                .ToList();

            var userReservation = reservationsByDate.Where(r => r.UserId == userId).FirstOrDefault();
            if (userReservation == null) return BadRequest("No User exist with this id");
            var patientNumber = userReservation.PatientNumber;
            var numberOfReservations = reservationsByDate.Count;
            var donePatients = reservationsByDate.Where(r => r.StatusId == 5).Count();

            return new GetDataOfReservationsDTO()
            {
                PatientNumber = patientNumber,
                NumberOfReservations = numberOfReservations,
                DonePatients = donePatients
            };


        }

        [Route("test/{clinicId}/{userId}/{date}")]
        [HttpGet]
        public async Task<ActionResult<GetDataOfReservationsV2DTO>> GetTeset(int clinicId, int userId, string date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            var clinic = await _context.Clinics.Where(c => c.Id == clinicId)
                .Include(c => c.Reservations).FirstOrDefaultAsync();
            var reservationsByDate = clinic.Reservations
                .Where(r => r.DateTime.ToShortDateString() == dateTime.ToShortDateString()
                && r.StatusId != 6)
                .ToList();

            var userReservation = reservationsByDate.Where(r => r.UserId == userId).FirstOrDefault();
            if (userReservation == null) return BadRequest("No User exist with this id");
            var patientNumber = userReservation.PatientNumber;
            var numberOfReservations = reservationsByDate.Count;
            var donePatients = reservationsByDate.Where(r => r.StatusId == 5).Count();
            var currentPatient = reservationsByDate.Where(r => r.StatusId == 4).FirstOrDefault();
            return new GetDataOfReservationsV2DTO()
            {
                PatientNumber = patientNumber,
                NumberOfReservations = numberOfReservations,
                DonePatients = donePatients,
                CurrentPatient = currentPatient != null ? currentPatient.PatientNumber : 0
            };

        }
    }
}