namespace QSApp.Entities
{
    public class ClinicAvailableDays
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        //public int? DoctorId { get; set; }
        public int WeekDayId { get; set; }
        public virtual WeekDay WeekDay { get; set; }
        //public virtual Doctor Doctor { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}
