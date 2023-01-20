using System.Collections.Generic;

namespace QSApp.DTOs
{
    public class TodayTomorrowResDTO
    {
        public IEnumerable<ReservationDTO> Today { get; set; }
        public IEnumerable<ReservationDTO> Tomorrow { get; set; }
    }
}
