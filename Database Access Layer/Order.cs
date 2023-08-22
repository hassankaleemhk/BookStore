using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database_Access_Layer
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; } // Primary Key
        [ForeignKey("UserId")]
        public int? UserId { get; set; } // Foreign Key to User

        public DateTime DateCreated { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int Phone_Number { get; set; }
        public string? Billing_Address { get; set; }
        public string Mailing_Address { get; set; }

        public virtual User User { get; set; } // Navigation property for User
        
        public virtual ICollection<OrderItem> OrderItems { get; set; } // One-to-many relationship with OrderItem
       
    }
}
