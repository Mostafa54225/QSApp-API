using QSApp.Entities;
using System;

namespace QSApp.DTOs
{
    public class ReservationDTODemo
    {
        public ReservationDTODemo()
        {
            Patient = new Patient();
            Clinic = new Clinic();
            Status = new Status();
        }
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime DateTime { get; set; }
        public int ClinicId { get; set; }
        public string QRCode { get; set; }
        public double Cost { get; set; }
        public int PatientNumber { get; set; }
        public int StatusId { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Clinic Clinic { get; set; }
        public virtual Status Status { get; set; }
    }
}
