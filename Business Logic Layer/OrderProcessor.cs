using Database_Access_Layer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer
{
    public class OrderProcessor
    {
        private readonly Dbcontext _context;

        public OrderProcessor(Dbcontext context)
        {
            _context = context;
        }
        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }
        public List<User> GetAllUsers(int? userid)
        {
            return _context.Users.Where(u => u.UserId == userid).ToList();
        }
        public Book GetBookById(int bookId)
        {
            return _context.Books.FirstOrDefault(b => b.BookId == bookId);
        }


        //public bool PlaceOrder(int? userId, OrderViewModel orderViewModel)
        //{

        //    // Check if the user exists
        //    var existingUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
        //    if (existingUser == null)
        //    {
        //        return false;
        //    }

        //    // Check if the order item is valid
        //    var book = _context.Books.FirstOrDefault(b => b.BookId == orderViewModel.BookId);
        //    if (book == null || book.InventoryCount < orderViewModel.Quantity)
        //    {
        //        return false;
        //    }

        //    // Create a new order
        //    var order = new Order
        //    {
        //        UserId = userId,
        //        DateCreated = DateTime.Now,
        //        TotalAmount = orderViewModel.UnitPrice * orderViewModel.Quantity,
        //        Status = "Pending",
        //        Billing_Address=orderViewModel.Billing_Address,
        //        Mailing_Address=orderViewModel.Mailing_Address ?? "",
        //        Phone_Number=orderViewModel.Phone_Number,
        //    };

        //    _context.Orders.Add(order);
        //    _context.SaveChanges(); // Save the changes to generate the OrderId

        //    // Set the order for the order item
        //    var orderItem = new OrderItem
        //    {
        //        OrderId = order.OrderId,
        //        BookId = orderViewModel.BookId,
        //        Quantity = orderViewModel.Quantity,
        //        UnitPrice = orderViewModel.UnitPrice
        //    };

        //    //_context.OrderItems.Add(orderItem);
        //    book.InventoryCount -= orderViewModel.Quantity;

        //    _context.SaveChanges(); // Save the changes to the book's inventory count

        //    return true;
        //}
        public bool PlaceOrder(int? userId, OrderViewModel orderViewModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Check if the user exists
                    //var existingUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
                    //if (existingUser == null)
                    //{
                    //    transaction.Rollback();
                    //    return false;
                    //}

                    // Create a new order
                    var order = new Order
                    {
                        UserId = userId,
                        DateCreated = DateTime.Now,
                        TotalAmount = orderViewModel.UnitPrice * orderViewModel.Quantity,
                        Status = "Pending",
                        Billing_Address = orderViewModel.Billing_Address,
                        Mailing_Address = orderViewModel.Mailing_Address ?? "",
                        Phone_Number = orderViewModel.Phone_Number,
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges(); // Save the changes to generate the OrderId

                    // Check if the order item is valid
                    var book = _context.Books.FirstOrDefault(b => b.BookId == orderViewModel.BookId);
                    if (book == null || book.InventoryCount < orderViewModel.Quantity)
                    {
                        transaction.Rollback();
                        _context.Orders.Remove(order); // Remove the order from the context if the order item is invalid
                        _context.SaveChanges();
                        return false;
                    }

                    // Set the order for the order item
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        BookId = orderViewModel.BookId,
                        Quantity = orderViewModel.Quantity,
                        UnitPrice = orderViewModel.UnitPrice
                    };

                    _context.OrderItems.Add(orderItem);
                    book.InventoryCount -= orderViewModel.Quantity;

                    _context.SaveChanges(); // Save the changes to the book's inventory count

                    // Check if both data is inserted successfully
                    if (!_context.ChangeTracker.HasChanges())
                    {
                        transaction.Rollback();
                        return false;
                    }

                    transaction.Commit(); // Commit the transaction if everything is successful
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Handle the exception, log the error, etc.
                    return false;
                }
            }
        }





        public void SaveOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
        //public void SaveOrders(IEnumerable<Order> orders)
        //{
        //    _context.Orders.AddRange(orders);
        //    _context.SaveChanges();
        //}




    }
}
