using Newtonsoft.Json;
using System;

namespace QSApp.Entities
{


    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]

    public class Reservation
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public DateTime DateTime { get; set; }
        public int ClinicId { get; set; }
        public int StatusId { get; set; }
        public string QRCode { get; set; }
        public int PatientNumber { get; set; }
        public DateTime ConfirmedDate { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Clinic Clinic { get; set; }
        public virtual Status Status { get; set; }

    }
}
