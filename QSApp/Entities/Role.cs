using System.Collections.Generic;

namespace QSApp.Entities
{
    public class Role
    {
        public Role()
        {
            Users = new List<User>();
        }
        public int Id { get; set; }
        public string RoleName { get; set; }
        public virtual IEnumerable<User> Users { get; set; }
    }
}
