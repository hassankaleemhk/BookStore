using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Access_Layer
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; } // Primary Key

        public string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; } // One-to-many relationship with User
    }

}
