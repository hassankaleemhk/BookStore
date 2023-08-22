using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Database_Access_Layer;
using Business_Logic_Layer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using BookStore.Views.Users;

namespace BookStore.Controllers
{
    [ServiceFilter(typeof(CustomExceptionFilterAttribute))] //exception filter

    //[CustomResultFilter] //result filter
    public class UsersController : Controller
    {
        
        private readonly UserSignupLogin _userSignupLogin;

        public UsersController(UserSignupLogin userSignupLogin)
        {

            _userSignupLogin = userSignupLogin;
        }

        // GET: Users
        [HttpGet]
        public IActionResult SignUp()
        {
            var userViewModel = new UserViewModel();
            return View(userViewModel);
        }

        [HttpPost]
        public IActionResult SignUp(UserViewModel userViewModel)
        {
            var user = new User
            {
                Username = userViewModel.Username,
                Password = userViewModel.Password,
                Email = userViewModel.Email,
                Name = userViewModel.Name,
                UserRoleId = null

            };

            bool isSignupSuccessful = _userSignupLogin.Signup(user);

            if (isSignupSuccessful)
            {
                return View(nameof(Login));
                //return RedirectToAction("CreateBook", "Inventory");
            }
            else
            {
                return View(userViewModel);
            }
        }
        
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var userViewModel = new UserViewModel();
            return View(userViewModel);
        }
        [HttpPost]
        public IActionResult ForgotPassword(UserViewModel userViewModel)
        {
            var user = _userSignupLogin.GetUserByEmail(userViewModel);
            if (user != null)
            {
                // Retrieve the password from the user object
                string password = user.Result.Password;
                if (password == null)
                {
                    ModelState.AddModelError(string.Empty, "Email not Found");
                }

                // Send the password via email
                try
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("hassan.kaleem@mammoth-ai.com"); // Specify the "From" address
                    mailMessage.To.Add(userViewModel.Email);
                    mailMessage.Subject = "Password Recovery";
                    mailMessage.Body = "Your password: " + password;

                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.Credentials = new System.Net.NetworkCredential("hassan.kaleem@mammoth-ai.com", "Lok19463");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);

                    // Redirect to a success page or show a success message
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Handle the case where the email sending fails
                    ModelState.AddModelError(string.Empty, "Failed to send email: " + ex.Message);
                    return View(userViewModel);
                }
            }
            else
            {
                // Handle the case where no user with the provided email is found
                ModelState.AddModelError(string.Empty, "Email not found");
                return View(userViewModel);
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            var userViewModel = new UserViewModel();
            return View(userViewModel);
        }
        [HttpPost]
        public IActionResult Login(UserViewModel userViewModel)
        {
            var user = _userSignupLogin.Login(userViewModel.Username, userViewModel.Password);
            if (user != null)
            {
                
                var userRole = _userSignupLogin.CheckUserRole(user);
                HttpContext.Session.SetInt32("UserId", user.UserId);

                HttpContext.Session.SetString("RoleName",userRole.RoleName);

                if (userRole == null || userRole.RoleName == "User")
                {
                    return Redirect("~/Order/SelectBook");
                }
                else if (userRole.RoleName == "Admin")
                {
                    return Redirect("~/Inventory/CreateBook");
                }
                else
                {
                    return Redirect("~/Inventory/ViewOrder");
                }
            }
            else
            {
                return View(userViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> UsersRolesAssign()
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            var userrole = HttpContext.Session.GetString("RoleName");
            if (userID != null && userrole == "Superuser")
            {
                var users = await _userSignupLogin.GetallUsers();

                var userViewModels = new List<UserViewModel>();

                foreach(var user in users)
                {
                    if (user.UserRoleId.HasValue)
                    {
                        //var userroles = await _userSignupLogin.GetUserRoleById(user.UserId.Value);
                        var userroles = await _userSignupLogin.GetUserRoleById(user.UserRoleId.Value);
                        var userViewModel = new UserViewModel
                        {
                            UserId = user.UserId,
                            Username = user.Username,
                            Email = user.Email,
                            Password = user.Password,
                            Name = user.Name,
                            UserRoles = userroles
                        };

                        userViewModels.Add(userViewModel);
                    }
                    else
                    {
                        var userViewModel = new UserViewModel
                        {
                            UserId = user.UserId,
                            Username = user.Username,
                            Email = user.Email,
                            Password = user.Password,
                            Name = user.Name,
                            UserRoles = new UserRole()
                        };

                        userViewModels.Add(userViewModel);
                    }
                }
                return View(userViewModels);
               
            }
            else
            {
                return Redirect("~/Users/AccessDenied");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UsersRolesAssign(int input1, string username, string email, string name)
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            var userrole = HttpContext.Session.GetString("RoleName");
            if (userID != null && userrole == "Superuser")
            {
                if (input1 != null || username != null || email != null || name != null)
                {
                    var userroles = await _userSignupLogin.GetUsersByDetails(input1,username,email,name);
                    var userViewModels = new List<UserViewModel>();
                    foreach(var user in userroles)
                    {
                        if (user.UserRoleId.HasValue)
                        {
                            var userroleid = await _userSignupLogin.GetUserRoleById(user.UserRoleId.Value);
                            var userViewModel = new UserViewModel
                            {
                                UserId =user.UserId,
                                Username= user.Username,
                                Email=user.Email,
                                Password=user.Password,
                                Name= user.Name,
                                UserRoles = userroleid,
                            };
                            userViewModels.Add(userViewModel);
                        }
                        else
                        {
                            var userViewModel = new UserViewModel
                            {
                                UserId =user.UserId,
                                Username = user.Username,
                                Email = user.Email,
                                Password = user.Password,
                                Name = user.Name,
                                UserRoles = new UserRole(),
                            };
                            userViewModels.Add(userViewModel);
                        }
                    }
                    return View(userViewModels);

                }
                else
                {
                    var users = await _userSignupLogin.GetallUsers();

                    var userViewModels = new List<UserViewModel>();

                    foreach (var user in users)
                    {
                        if (user.UserRoleId.HasValue)
                        {
                            //var userroles = await _userSignupLogin.GetUserRoleById(user.UserId.Value);
                            var userroles = await _userSignupLogin.GetUserRoleById(user.UserRoleId.Value);
                            var userViewModel = new UserViewModel
                            {
                                UserId = user.UserId,
                                Username = user.Username,
                                Email = user.Email,
                                Password = user.Password,
                                Name = user.Name,
                                UserRoles = userroles
                            };

                            userViewModels.Add(userViewModel);
                        }
                        else
                        {
                            var userViewModel = new UserViewModel
                            {
                                UserId = user.UserId,
                                Username = user.Username,
                                Email = user.Email,
                                Password = user.Password,
                                Name = user.Name,
                                UserRoles = new UserRole()
                            };

                            userViewModels.Add(userViewModel);
                        }
                    }
                    return View(userViewModels);
                }
            }
            else
            {
                return Redirect("~/Users/Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userSignupLogin.GetUserById((int)id);
            if (user == null)
            {
                return NotFound();
            }

            var role = await _userSignupLogin.GetUserRoleById((int)id);

            var userViewModel = new UserViewModel
            {
                UserRoles = role,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                Name = user.Name
            };

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, UserViewModel userViewModel)
        {

            if (id == null)
            {
                return NotFound();
            }
            var users = await _userSignupLogin.GetUserById((int)id);
            users.UserRoleId = userViewModel.UserRoleId;

            if (_userSignupLogin.UpdateUserRole(users))
            {
               return RedirectToAction("UsersRolesAssign"); // Redirect to the appropriate action
            }
            else
            {
              return NotFound();
            }
            

        }
        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
