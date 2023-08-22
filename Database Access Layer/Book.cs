using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Database_Access_Layer
{
    public class Book
    {
        [Key]
        public int BookId { get; set; } // Primary Key
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int InventoryCount { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } // One-to-many relationship with OrderItem
    }
}