using System.Collections.Generic;

namespace QSApp.Entities
{
    public class WeekDay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<ClinicAvailableDays> ClinicAvaliableDay { get; set; }
    }
}
