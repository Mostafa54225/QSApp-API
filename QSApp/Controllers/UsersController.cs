using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QSApp.DataContext;
using QSApp.DTOs;
using QSApp.Entities;
using QSApp.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QSApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly QsDbContext _context;


        public UsersController(QsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _context
                       .Users
                       .Include(x => x.Role)
                       .Select(u => new UserDTO()
                       {
                           Id = u.Id,
                           Password = u.Password,
                           PhoneNumber = u.PhoneNumber,
                           UserName = u.UserName,
                           RoleId = u.Role.Id,
                           Role = u.Role.RoleName,
                           NationalId = u.NationalId,
                           Latitude = u.Latitude,
                           Longitude = u.Longitude,
                           PatientCode = u.Role.Id == 1 ? _context.Patients
                           .Where(p => p.UserId == u.Id).FirstOrDefault().PatientGeneratedNumber : null
                       })
                       .ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(UserDTO user)
        {
            var validation = validateUser(user);

            if (!validation.result)
                return BadRequest(validation.message);

            user.Password = AesOperation.EncryptString(user.Password);

            var addedUser = _context.Users.Add(new User()
            {
                UserName = user.UserName,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                NationalId = user.NationalId,
                Longitude = user.Longitude,
                Latitude = user.Latitude
            });

            if (_context.SaveChanges() > 0)
            {
                if (user.RoleId == 1)
                {
                    var patients = _context.Patients.ToList();
                    var NoOfPatients = patients.Count;
                    var uniqueId = "000000";
                    if (NoOfPatients == 0)
                    {
                        uniqueId = GetUniqueId("000000");
                    }
                    else
                    {
                        var lastPatient = _context.Patients.OrderByDescending(p => p.Id).FirstOrDefault();
                        uniqueId = GetUniqueId(lastPatient.PatientGeneratedNumber);
                    }
                    if (_context.Patients.Any(x => x.PatientGeneratedNumber == uniqueId))
                        return BadRequest();
                    _context.Patients.Add(new Patient()
                    {
                        UserId = addedUser.Entity.Id,
                        PatientGeneratedNumber = uniqueId
                    });
                    _context.SaveChanges();
                }
            }

            return Ok($"{user.UserName} was added successfully");
        }
        private string GetUniqueId(string uniqueId)
        {
            int newNumber = Convert.ToInt32(uniqueId) + 1;
            return uniqueId = newNumber.ToString("000000");
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserDTO user)
        {
            if (user == null)
                return BadRequest("Sorry, Data Can't be Null");

            if (
                 (
                   (string.IsNullOrEmpty(user.UserName))
                   & (string.IsNullOrEmpty(user.PhoneNumber))
                   & (string.IsNullOrEmpty(user.NationalId))
                 )

                || string.IsNullOrEmpty(user.Password)
               )
                return BadRequest("Some fields can't be empty");

            var loggedUser = _context
                .Users
                .Include(x => x.Role)
                .Where(x =>
                        (x.UserName == user.UserName || x.PhoneNumber == user.PhoneNumber || x.NationalId == user.NationalId)
                        && x.Password == AesOperation.EncryptString(user.Password))
                        .FirstOrDefault();

            if (loggedUser is not null)
            {
                IEnumerable<ReservationDTO> reservations = null;
                if (loggedUser.RoleId == 1)
                {
                    reservations = GetPatientReservations(loggedUser.Id);
                }
                var client = new RestClient("https://dev-6kxf0yk8.us.auth0.com/oauth/token");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "{\"client_id\":\"kXYsokiGiDmB8Vfa4u5RigJUXl6Zn0tB\",\"client_secret\":\"ikfSrZygqu8JWgOhPRs9ry2l0sCtXPPi9Z_zh5Hp5Yt6Wfxa_BoPpklXAAOdPnQ7\",\"audience\":\"http://qsapp.somee.com\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
                RestResponse response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<dynamic>(response.Content);



                return new JsonResult(new LoginResponseDTO()
                {
                    AccessToken = result.access_token,
                    NationalId = loggedUser.NationalId,
                    UserId = loggedUser.Id,
                    PhoneNumber = loggedUser.PhoneNumber,
                    Latitude = loggedUser.Latitude,
                    Longitude = loggedUser.Longitude,
                    RoleId = loggedUser.RoleId,
                    RoleName = loggedUser.Role.RoleName,
                    UserName = loggedUser.UserName,
                    PatientCode = loggedUser.RoleId == 1 ? _context.Patients
                    .Where(p => p.UserId == loggedUser.Id).FirstOrDefault().PatientGeneratedNumber : null,
                    Reservations = reservations?.Select(r => new ReservationDTO()
                    {
                        Id = r.Id,
                        PatientId = r.PatientId,
                        DateTime = r.DateTime,
                        ClinicId = r.ClinicId,
                        UserId = loggedUser.Id,
                        Status = r.Status,
                        StatusId = r.StatusId,
                        PatientNumber = r.PatientNumber,
                        ClinicName = r.ClinicName,
                        Cost = r.Cost,
                        PhoneNumber = loggedUser.PhoneNumber,
                        PatientName = loggedUser.UserName,
                        ConfirmedDate = r.ConfirmedDate
                    })
                });
            }

            return BadRequest("Sorry, something went wrong");
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<UserDTO> GetUser(int id)
        {
            var p = await _context.Users
            .Where(u => u.Id == id).Select(u => new UserDTO()
            {
                Id = u.Id,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                Role = u.Role.RoleName,
                NationalId = u.NationalId,
                Longitude = u.Longitude,
                Latitude = u.Latitude,
                PatientCode = _context.Patients.Where(p => p.UserId == id).FirstOrDefault().PatientGeneratedNumber
            }).FirstOrDefaultAsync();
            return p;
        }

        private (bool result, string message) validateUser(UserDTO user)
        {
            if (user == null)
                return (false, "Sorry, Data Can't be empty");

            #region UserName
            if (string.IsNullOrEmpty(user.UserName))
                return (false, "Sorry, User Name can't be empty");

            if (_context.Users.Any(x => x.UserName == user.UserName))
                return (false, "Sorry, User Name is already exist");
            #endregion

            #region Password
            if (string.IsNullOrEmpty(user.Password))
                return (false, "Sorry, User password can't be empty");

            if (user.Password.Length < 5)
                return (false, "Sorry, Password must be at least 5 characters, Note: numbers and symbols are valid");
            #endregion

            #region PhoneNumber
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return (false, "Sorry, User PhoneNumber can't be empty");
            if (_context.Users.Any(x => x.PhoneNumber == user.PhoneNumber))
                return (false, "Sorry, this phone number is already exist");

            #endregion

            #region Roles
            if (user.RoleId == 0)
                return (false, "Sorry, Please enter User type");
            #endregion

            #region nationalId
            if (string.IsNullOrEmpty(user.NationalId))
                return (false, "Sorry, Please enter National Id");
            if (!user.NationalId.All(char.IsDigit))
                return (false, "Sorry, Please Note that national Id must be digits only");
            if (user.NationalId.Length != 14)
                return (false, "Sorry, Please Note that national Id must be 14 digits");
            if (_context.Users.Any(x => x.NationalId == user.NationalId))
                return (false, "Sorry, this national id is already exist");
            #endregion

            return (true, "");
        }

        private IEnumerable<ReservationDTO> GetPatientReservations(int userId) =>
            _context.Reservations.Where(r => r.Patient.UserId == userId
            && r.DateTime >= DateTime.Today).Select(r => new ReservationDTO()
            {
                Id = r.Id,
                PatientId = r.PatientId,
                DateTime = r.DateTime.ToShortDateString(),
                ClinicId = r.ClinicId,
                StatusId = r.StatusId,
                QRCode = r.QRCode,
                //Cost = r.Cost,
                ClinicName = r.Clinic.Name,
                PatientNumber = r.PatientNumber,
                Status = r.Status.Name,
                PhoneNumber = r.Patient.User.PhoneNumber,
                ConfirmedDate = r.ConfirmedDate.ToShortDateString()
            }).ToList();
    }
}
