using System;

namespace QSApp.DTOs
{
    public class ReservationDTORequestBody
    {
        public int UserId { get; set; }
        public int ClinicId { get; set; }
        public DateTime DateTime { get; set; }

    }
}
