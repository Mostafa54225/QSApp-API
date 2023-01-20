using System.Collections.Generic;

namespace QSApp.Entities
{
    public class Patient
    {
        public Patient()
        {
            //Reservations = new List<Reservation>();
            //User = new User();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PatientGeneratedNumber { get; set; }
        public virtual IEnumerable<Reservation> Reservations { get; set; }
        public virtual User User { get; set; }
    }
}
