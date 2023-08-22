using Database_Access_Layer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer
{
    public class OrderViewModel
    {
        public int OrderId { get; set; } // Primary Key
        //[ForeignKey("UserId")]
        public int? UserId { get; set; } // Foreign Key to User

        public DateTime DateCreated { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public virtual User User { get; set; } // Navigation property for User
        //public int OrderItemId { get; set; } // Primary Key
        //[ForeignKey("BookId")]
        public int BookId { get; set; } // Foreign Key to Book
        [Required]
        public string Name { get; set; }
        [Required]
        public string Title { get; set; }
        [Required] public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        [Required] public int Phone_Number { get; set; }
        [Required] public string? Billing_Address { get; set; }
        [Required] public string Mailing_Address { get; set; }

       public virtual Order Order { get; set; } // Navigation property for Order
        public virtual OrderItem OrderItem { get; set; }

        //public virtual Book Book { get; set; } // Navigation property for Book

    }
}
