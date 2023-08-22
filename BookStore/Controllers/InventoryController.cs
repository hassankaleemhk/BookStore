using Business_Logic_Layer;
using Database_Access_Layer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryManager _inventoryManager;

        public InventoryController(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }


        // GET: /Inventory/Create
        [HttpGet]
        public IActionResult CreateBook()
        {
            
            var adminID = HttpContext.Session.GetInt32("UserId");
            var userrole = HttpContext.Session.GetString("RoleName");
            if (adminID != null && userrole =="Admin" || userrole =="Superuser")
            {
                return View();
            }
            else
            {
                return Redirect("~/Users/Login");
            }
  
        }

        // POST: /Inventory/Create
        //[ValidateAntiForgeryToken]
        [HttpPost]
        
        public IActionResult CreateBook(BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                //var adminID = HttpContext.Session.GetInt32("UserId");
                var book = new Book
                {
                    Title = bookViewModel.Title,
                    Author = bookViewModel.Author,
                    Price = bookViewModel.Price,
                    InventoryCount = bookViewModel.InventoryCount
                    
                };

                if (_inventoryManager.AddBook(book))
                {
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    ModelState.AddModelError("", "The book already exists in the inventory.");
                }
            }

            return View(bookViewModel);
        }
      

        [HttpGet]
        public async Task<IActionResult> ViewOrder(string input1,DateTime startdate, DateTime enddate)
        {
            
            var userID = HttpContext.Session.GetInt32("UserId");
            var userrole = HttpContext.Session.GetString("RoleName");
            if (userID != null && userrole == "Superuser")
            {
                if(startdate !=null && enddate != null && startdate != DateTime.MinValue || input1 != null)
                {
                    var orderdate = await _inventoryManager.GetOrdersByDate((string)input1, startdate,enddate);
                    var orderViewModels = new List<OrderViewModel>();

                    foreach (var order in orderdate)
                    {
                        if (order.UserId.HasValue)
                        {
                            var user = await _inventoryManager.GetUserById(order.UserId.Value);
                            var orderViewModel = new OrderViewModel
                            {
                                OrderId = order.OrderId,
                                User = user,
                                DateCreated = order.DateCreated,
                                TotalAmount = order.TotalAmount,
                                Status = order.Status
                            };

                            orderViewModels.Add(orderViewModel);
                        }
                        else
                        {
                            // Handle the case when order.UserId is null
                            // You can choose to skip this order or assign a default user
                            // For example:
                            var orderViewModel = new OrderViewModel
                            {
                                OrderId = order.OrderId,
                                User = new User(), // Provide a default user or null
                                DateCreated = order.DateCreated,
                                TotalAmount = order.TotalAmount,
                                Status = order.Status
                            };

                            orderViewModels.Add(orderViewModel);
                        }
                    }

                    return View(orderViewModels);
                }
                else
                {
                    var orders = await _inventoryManager.GetOrders();

                    var orderViewModels = new List<OrderViewModel>();

                    foreach (var order in orders)
                    {
                        if (order.UserId.HasValue)
                        {
                            var user = await _inventoryManager.GetUserById(order.UserId.Value);
                            var orderViewModel = new OrderViewModel
                            {
                                OrderId = order.OrderId,
                                User = user,
                                DateCreated = order.DateCreated,
                                TotalAmount = order.TotalAmount,
                                Status = order.Status
                            };

                            orderViewModels.Add(orderViewModel);
                        }
                        else
                        {
                            // Handle the case when order.UserId is null
                            // You can choose to skip this order or assign a default user
                            // For example:
                            var orderViewModel = new OrderViewModel
                            {
                                OrderId = order.OrderId,
                                User = new User(), // Provide a default user or null
                                DateCreated = order.DateCreated,
                                TotalAmount = order.TotalAmount,
                                Status = order.Status
                            };

                            orderViewModels.Add(orderViewModel);
                        }
                    }

                    return View(orderViewModels);
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

            var order = await _inventoryManager.GetOrdersByID((int)id);
            if (order == null)
            {
                return NotFound();
            }

            var user = await _inventoryManager.GetUserById((int)order.UserId);

            var orderViewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                User = user,
                DateCreated = order.DateCreated,
                TotalAmount = order.TotalAmount,
                Status = order.Status
            };

            return View(orderViewModel);
        }
     


        [HttpPost]
        public async Task<IActionResult> Edit(int? id, OrderViewModel orderViewModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _inventoryManager.GetOrdersByID((int)id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = orderViewModel.Status;

            if (_inventoryManager.UpdateOrderStatus(order))
            {
                return RedirectToAction("ViewOrder");
            }

            return RedirectToAction("ViewOrder");
        }
    }
}
