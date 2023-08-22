using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database_Access_Layer
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; } // Primary Key
        [ForeignKey("OrderId")]
        public int OrderId { get; set; } // Foreign Key to Order
        [ForeignKey("BookId")]
        public int BookId { get; set; } // Foreign Key to Book
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Order Order { get; set; } // Navigation property for Order

        public virtual Book Book { get; set; } // Navigation property for Book
        
    }
}
