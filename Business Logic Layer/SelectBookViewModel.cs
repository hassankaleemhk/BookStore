using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer
{
    public class SelectBookViewModel
    {
        public List<BookViewModel> Books { get; set; }
        public ShoppingCartViewModel ShoppingCart { get; set; }
    }
}
