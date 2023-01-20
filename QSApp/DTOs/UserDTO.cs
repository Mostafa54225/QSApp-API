
using Newtonsoft.Json;

namespace QSApp.DTOs
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public string Role { get; set; }
        public string NationalId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PatientCode { get; set; }
    }
}
