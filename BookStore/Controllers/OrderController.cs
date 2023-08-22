using Business_Logic_Layer;
using Database_Access_Layer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Net;


namespace BookStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderProcessor _processor;
        public OrderController(OrderProcessor processor)
        {
            _processor = processor;
        }


        [HttpGet]
        public ActionResult Index(int bookId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                var book = _processor.GetBookById(bookId);
                if (book == null)
                {
                    // Handle book not found error
                    return Content("Book not found");
                }

                var bookViewModel = new BookViewModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    Price = book.Price
                };

                ViewData["Books"] = new List<BookViewModel> { bookViewModel };
                ViewData["Users"] = _processor.GetAllUsers(userId)
                    .Select(a => new SelectListItem { Value = a.UserId.ToString(), Text = a.Name })
                    .ToList();

                var orderViewModel = new OrderViewModel();
                return View(orderViewModel);
            }
        }
       
        [HttpGet]
        public ActionResult GetBookPrice(int bookId)
        {
            var book = _processor.GetBookById(bookId);
            if (book != null)
            {
                var price = book.Price;
                return Content(price.ToString());
            }

            return Content("Book not found");
        }

        [HttpPost]
        public ActionResult Index(OrderViewModel model)
        {
            
                // Add the selected book to the shopping cart
                var shoppingCart = HttpContext.Session.GetObject<ShoppingCartViewModel>("ShoppingCart");
                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCartViewModel();
                }

                var orderViewModel = new OrderViewModel
                {
                    BookId = model.BookId,
                    Title = model.Title,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice,
                    TotalAmount = model.TotalAmount,
                    Phone_Number = model.Phone_Number,
                    Billing_Address = model.Billing_Address,
                    Mailing_Address = model.Mailing_Address,

                };

                shoppingCart.Orders.Add(orderViewModel);

                // Update the shopping cart in the session
                HttpContext.Session.SetObject("ShoppingCart", shoppingCart);

                //return RedirectToAction("SelectBook");
                return Redirect("~/Order/SelectBook");
        }

        
        [HttpGet]
        public ActionResult SelectBook()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {

                var books = _processor.GetAllBooks()
                    .Select(a => new BookViewModel
                    {
                        BookId = a.BookId,
                        Title = a.Title,
                        Author = a.Author,
                        Price = a.Price
                    })
                    .ToList();

                ViewData["Books"] = books;

                ViewData["Users"] = _processor.GetAllUsers(userId)
                    .Select(a => new SelectListItem { Value = a.UserId.ToString(), Text = a.Name })
                    .ToList();

                // Get the shopping cart from the session
                var shoppingCart = HttpContext.Session.GetObject<ShoppingCartViewModel>("ShoppingCart");
                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCartViewModel();
                }

                return View(new SelectBookViewModel
                {
                    Books = books,
                   
                    ShoppingCart = shoppingCart
                });
            }
        }

        
        [HttpPost]
        public IActionResult SelectBook(int bookId, string submitButton)
        {
            switch (submitButton)
            {
                case "placeorder":
                    {
                        // Delegate sending to another controller action

                        var userId = HttpContext.Session.GetInt32("UserId");

                        // Get the shopping cart from the session
                        var shoppingCart = HttpContext.Session.GetObject<ShoppingCartViewModel>("ShoppingCart");
                        if (shoppingCart == null)
                        {
                            shoppingCart = new ShoppingCartViewModel();
                        }

                        // Add the selected book to the shopping cart
                        var selectedBook = _processor.GetBookById(bookId);
                        if (selectedBook != null)
                        {
                            var orderViewModel = new OrderViewModel
                            {
                                BookId = selectedBook.BookId,
                                Title = selectedBook.Title,
                                Quantity = 1, // Set the initial quantity to 1
                                UnitPrice = selectedBook.Price
                            };

                            // Deserialize the JSON string from the shopping cart and assign it to orderViewModel
                            var cartItem = shoppingCart.Orders.FirstOrDefault(o => o.BookId == bookId);
                            if (cartItem != null)
                            {
                                orderViewModel.Billing_Address = cartItem.Billing_Address;
                                orderViewModel.Mailing_Address = cartItem.Mailing_Address;
                                orderViewModel.Phone_Number = cartItem.Phone_Number;
                            }

                            shoppingCart.Orders.Add(orderViewModel);
                        }

                        bool ordersPlaced = false;
                        decimal totalAmount = 0; // Initialize total amount to 0
                        foreach (var orderViewModel in shoppingCart.Orders)
                        {
                            bool orderPlaced = _processor.PlaceOrder(userId, orderViewModel);
                            if (orderPlaced)
                            {
                                ordersPlaced = true;
                                totalAmount += orderViewModel.UnitPrice * orderViewModel.Quantity; // Calculate the total amount for each order
                            }
                            else
                            {
                                // Handle the case where the order placement fails
                                ordersPlaced = false;
                                break;
                            }
                        }

                        if (ordersPlaced)
                        {
                            // Create a single order with the total amount for all items
                            var order = new Order
                            {
                                UserId = userId,
                                TotalAmount = totalAmount
                            };

                            // Save the order to the database (assuming you have a mechanism to save orders)
                            //_processor.SaveOrder(order);

                            shoppingCart.Orders.Clear(); // Clear the shopping cart after placing orders successfully
                            HttpContext.Session.SetObject("ShoppingCart", shoppingCart);
                            //return Json(new { success = true }); // Return success response
                            return Redirect("~/Order/OrderConfirmation");
                        }
                        else
                        {
                            shoppingCart.Orders.Clear(); // Clear the shopping cart after placing orders successfully
                            HttpContext.Session.SetObject("ShoppingCart", shoppingCart);
                            // Handle the case where order placement fails for any of the books
                            return Json(new { success = false }); // Return failure response
                        }
                    }
                case "remove":
                    // Call another action to perform the cancellation
                    return RemoveOrder(bookId);
                default:
                    return Json(new { success = false });
            }
        }




        [HttpGet]
        public ActionResult OrderConfirmation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RemoveOrder(int bookId)
        {

            var shoppingCart = HttpContext.Session.GetObject<ShoppingCartViewModel>("ShoppingCart");
            if (shoppingCart != null)
            {

                var orderToRemove = shoppingCart.Orders.FirstOrDefault(o => o.BookId == bookId);
                if (orderToRemove != null)
                {

                    shoppingCart.Orders.Remove(orderToRemove);
                }
            }


            HttpContext.Session.SetObject("ShoppingCart", shoppingCart);


            //return Json(new { success = true });
            return Redirect("~/Order/SelectBook");

        }
        public ActionResult MyAction(string submitButton)
        {
            switch (submitButton)
            {
                case "placeorder":
                    // delegate sending to another controller action
                    return (SelectBook());
                case "remove":
                    // call another action to perform the cancellation
                    return (View());
                default:
                    return (View());
            }

        }
        }
}
