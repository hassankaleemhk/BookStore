using Database_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer
{
    public class UserViewModel
    {
       
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string RoleName { get; set; }
            public int? UserRoleId { get; set; }
            public virtual UserRole UserRoles { get; set; }
    }
}
