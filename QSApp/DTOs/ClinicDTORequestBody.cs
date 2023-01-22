using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace QSApp.DTOs
{
    public class ClinicDTORequestBody
    {
        public string Name { get; set; }
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
        public string LogoFile { get; set; }
        public string Logo { get; set; }
        public int Limit { get; set; }
        public string ClinicType { get; set; }
        public IEnumerable<string> WorkDays { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
