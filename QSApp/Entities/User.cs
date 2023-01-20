namespace QSApp.Entities
{
    public class User
    {
        public User()
        {
            //Role = new Role();
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }
        public string NationalId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
