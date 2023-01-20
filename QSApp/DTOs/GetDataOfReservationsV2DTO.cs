namespace QSApp.DTOs
{
    public class GetDataOfReservationsV2DTO
    {
        public int PatientNumber { get; set; }
        public int NumberOfReservations { get; set; }
        public int DonePatients { get; set; }
        public int CurrentPatient { get; set; }
    }
}
