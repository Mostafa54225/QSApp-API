using Microsoft.EntityFrameworkCore;
using QSApp.Entities;
using System.Collections.Generic;

namespace QSApp.DataContext
{
    public class QsDbContext : DbContext
    {
        public QsDbContext(DbContextOptions<QsDbContext> options)
            : base(options)
        {

        }

        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<ClinicAvailableDays> ClinicAvaliableDays { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<WeekDay> WeekDays { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeekDay>().HasData(populateWeekDays());

            modelBuilder.Entity<Role>().HasData(populateRoles());



            modelBuilder.Entity<Status>().HasData(populateStatus());
        }

        private List<WeekDay> populateWeekDays() =>
            new List<WeekDay>
            {
                new WeekDay
                {
                    Id = 1,
                    Name="Saturday"
                },
                new WeekDay
                {
                    Id=2,
                    Name="Sunday"
                },
                new WeekDay
                {
                    Id=3,
                    Name="Monday"
                },
                new WeekDay
                {
                    Id=4,
                    Name="Tuesday"
                },
                new WeekDay
                {
                    Id=5,
                    Name="Wednesday"
                },
                new WeekDay
                {
                    Id=6,
                    Name="Thursday"
                },
                new WeekDay
                {
                    Id=7,
                    Name="Friday"
                }
            };

        private List<Role> populateRoles() =>
            new List<Role>()
            {
                new()
                {
                    Id=1,
                    RoleName="Patient"
                }
                ,
                new()
                {
                   Id=2,
                   RoleName="Nursing"
                },
                new()
                {
                   Id=3,
                   RoleName="Doctor"
                },
                new()
                {
                    Id=4,
                    RoleName="Admin"
                },
            };

        private List<Status> populateStatus() =>
            new List<Status>()
            {
                new()
                {
                    Id=1,
                    Name="NotConfirmed"
                },
                new()
                {
                    Id=2,
                    Name="Confirmed"
                },
                new()
                {
                    Id=3,
                    Name="Waiting"
                },
                new()
                {
                    Id=4,
                    Name="Current"
                },
                new()
                {
                    Id=5,
                    Name="Done"
                },
                new()
                {
                    Id=6,
                    Name="Canceled"
                }
            };
    }
}