using Newtonsoft.Json;
using System.Collections.Generic;

namespace QSApp.DTOs
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ReservationDTO> Reservations { get; set; }
        public string AccessToken { get; set; }
        public string PatientCode { get; set; }
    }
}
