using System.ComponentModel.DataAnnotations;

namespace QSApp.DTOs
{
    public class ClinicAvaliableDayDTO
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public int DoctorId { get; set; }
        [MaxLength(50)]
        public string Day { get; set; }
    }
}
