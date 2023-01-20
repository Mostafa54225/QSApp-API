using System.Collections.Generic;

namespace QSApp.DTOs
{
    public class NoOfReservationsClinicsDTO
    {
        public IEnumerable<IDictionary<string, int>> ClinicsResevations { get; set; }
    }
}
