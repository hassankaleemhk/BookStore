using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Database_Access_Layer
{
    public class User
    {
        [Key]
        public int UserId { get; set; } // Primary Key
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        [ForeignKey("UserRoleId")]
        public int? UserRoleId { get; set; } // Foreign Key
        //public virtual UserRole UserRole { get; set; } // Navigation property for UserRole

        public virtual ICollection<Order> Orders { get; set; } // One-to-many relationship with Order
    }
}
