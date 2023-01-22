using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace QSApp.DTOs
{
    public class ClinicDTO
    {
        public ClinicDTO()
        {
            //AvaliableDates = new List<string>();
            //Reservations = new List<Reservation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
        public string LogoFile { get; set; }
        public string Logo { get; set; }
        public int Limit { get; set; }
        public string ClinicType { get; set; }
        public double Cost { get; set; }

        public IEnumerable<string> AvaliableDates { get; set; }

        public IEnumerable<string> WorkDays { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual IEnumerable<ReservationDTO> Reservations { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual IEnumerable<DoctorDTO> Doctors { get; set; }

    }
}