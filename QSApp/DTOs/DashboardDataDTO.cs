using System.Collections.Generic;



namespace QSApp.DTOs
{
    public class DashboardDataDTO
    {
        public int Users { get; set; }
        public int Patients { get; set; }
        public int Clinics { get; set; }
        public int Reservations { get; set; }
        public IDictionary<string, int> NumberOfReservationsPerMonth { get; set; }
    }
}
