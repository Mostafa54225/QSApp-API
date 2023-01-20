using System.Collections.Generic;

namespace QSApp.DTOs
{
    public class ClinicDTOUser
    {
        public string Name { get; set; }
        public IEnumerable<string> AvaliableDates { get; set; }
        //public IEnumerable<string> WorkDays { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
