using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer
{
    public class ShoppingCartViewModel
    {
        public ShoppingCartViewModel()
        {
            Orders = new List<OrderViewModel>();
        }
        public List<OrderViewModel> Orders { get; set; }
    }
}
