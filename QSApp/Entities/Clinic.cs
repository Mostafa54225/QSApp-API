using System.Collections.Generic;

namespace QSApp.Entities
{
    public class Clinic
    {
        public Clinic()
        {
            Reservations = new List<Reservation>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClinicType { get; set; }
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
        public string Logo { get; set; }
        public int Limit { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Cost { get; set; }
        public virtual IEnumerable<Reservation> Reservations { get; set; }
        public virtual IEnumerable<ClinicAvailableDays> ClinicAvaliableDays { get; set; }
    }
}