using System.Collections.Generic;

namespace QSApp.Entities
{
    public class Status
    {
        public Status()
        {
            Reservations = new List<Reservation>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<Reservation> Reservations { get; set; }
    }
}
