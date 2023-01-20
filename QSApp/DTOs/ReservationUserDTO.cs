namespace QSApp.DTOs
{
    public class ReservationUserDTO
    {
        public int Id { get; set; }
        public string DateTime { get; set; }
        public string ConfirmedDate { get; set; }
        public double Cost { get; set; }
        public int PatientNumber { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string PatientName { get; set; }
        public string ClinicName { get; set; }
        public int ClinicId { get; set; }

    }
}
