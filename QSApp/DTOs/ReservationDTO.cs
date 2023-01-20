using Newtonsoft.Json;
using QSApp.Entities;

namespace QSApp.DTOs
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public int ClinicId { get; set; }
        public string DateTime { get; set; }
        public string QRCode { get; set; }
        public double Cost { get; set; }
        public int PatientNumber { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string ConfirmedDate { get; set; }
        public string PhoneNumber { get; set; }
        public string PatientName { get; set; }
        public string NationalId { get; set; }
        public string ClinicName { get; set; }
        public virtual Clinic Clinic { get; set; }
        //public virtual Status Status { get; set; }
    }
}
