using Database_Access_Layer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer
{
    public class UserSignupLogin
    {

        private readonly Dbcontext _context;

        public UserSignupLogin(Dbcontext context)
        {
            _context = context;
        }

        public bool Signup(User user)
        {
            // Check if the username is already taken
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return false;
            }

            // Add the new user to the database
            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }
        //public bool CheckSuperuser(User user)
        //{
        //    var superuser = _context.Users.FirstOrDefault(u => u.Name == user.Name && u.Superuser == user.Superuser);
        //    if (superuser = true)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}
        //public bool CheckSuperuser(User user)
        //{
        //    bool superuser = _context.Users.Any(u => u.Name == user.Name && u.Superuser == true);
        //    return superuser;
        //}


        //public User Login(string username, string password)
        //{
        //    // Check if the user exists and the password matches

        //    var existingUser = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        //    return existingUser;
        //}
        public UserRole CheckUserRole(User user)
        {
            if (user.UserRoleId == null)
            {
                return null;
            }

            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserRoleId == user.UserRoleId);
            return userRole;
        }
        public User Login(string username, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            return existingUser;
        }
        public async Task<List<User>> GetallUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }
        public async Task<UserRole> GetUserRoleById(int Id)
        {
            var userrole = await _context.UserRoles.FindAsync(Id);
            return userrole;
        }
        public async Task<User> GetUserByEmail(UserViewModel userViewModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userViewModel.Email);
            return user;
        }

        public bool UpdateUserRole(User user)
        {
            var existingOrder = _context.Users.Find(user.UserId);
            if (existingOrder != null)
            {
                existingOrder.UserRoleId = user.UserRoleId;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<List<User>> GetUsersByDetails(int? input1, string username, string email, string name)
        {
            var userQuery = _context.Users.AsQueryable();

            if (input1 > 0)
            {
                userQuery = userQuery.Where(user => user.UserRoleId == input1.Value);
            }

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(name))
            {
                userQuery = userQuery.Where(user => user.Username == username || user.Username == username || user.Email == email || user.Name == name);
            }
            //if (!string.IsNullOrEmpty(email))
            //{
            //    userQuery = userQuery.Where(user => user.Email == email);
            //}
            //if (!string.IsNullOrEmpty(name))
            //{
            //    userQuery = userQuery.Where(user => user.Name == name);
            //}

            var users = await userQuery.ToListAsync();

            return users;
        }






    }
}

