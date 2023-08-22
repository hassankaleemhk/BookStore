using Database_Access_Layer;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer
{
    public class InventoryManager
    {
        private readonly Dbcontext _context;

        public InventoryManager(Dbcontext context)
        {
            _context = context;
        }
        //public List<Admin> GetAllAdmins()
        //{
        //    return _context.Admins.ToList();
        //}



        public bool AddBook(Book book)
        {
            // Check if the book already exists
            if (_context.Books.Any(b => b.Title == book.Title && b.Author == book.Author))
            {
                return false;
            }

            // Add the new book to the database
            _context.Books.Add(book);
            _context.SaveChanges();

            return true;
        }

        public bool UpdateBook(int bookId, Book book)
        {
            // Find the book in the database
            var existingBook = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (existingBook == null)
            {
                return false;
            }

            // Update the book details
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Price = book.Price;
            existingBook.InventoryCount = book.InventoryCount;

            _context.SaveChanges();

            return true;
        }

        public bool RemoveBook(int bookId)
        {
            // Find the book in the database
            var existingBook = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (existingBook == null)
            {
                return false;
            }

            // Remove the book from the database
            _context.Books.Remove(existingBook);
            _context.SaveChanges();

            return true;
        }
        public async Task<List<Order>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return orders;
        }
        //To get user name in Orders Management View
        public async Task<User> GetUserById(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user;
        }
        public async Task<Order> GetOrdersByID(int Id)
        {
            var order = await _context.Orders.FindAsync(Id);
            return order;
        }
        public async Task<List<Order>> GetOrdersByStatus(string inputStatus)
        {
            var orders = await _context.Orders.Where(order => order.Status == inputStatus).ToListAsync();
            return orders;
        }

        //public async Task<List<Order>> GetOrdersByDate(string input1, DateTime startdate, DateTime enddate)
        //{ 
        //    enddate = enddate.AddDays(1);
        //    var orders = await _context.Orders.Where(order => order.Status == input1 || order.DateCreated >= startdate && order.DateCreated < enddate).ToListAsync();

        //    return orders;
        //}
        public async Task<List<Order>> GetOrdersByDate(string input1, DateTime? startdate, DateTime? enddate)
        {
            enddate = enddate?.AddDays(1);
            var ordersQuery = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(input1))
            {
                ordersQuery = ordersQuery.Where(order => order.Status == input1);
            }

            if (startdate.HasValue && enddate.HasValue)
            {
                ordersQuery = ordersQuery.Where(order => order.DateCreated >= startdate.Value && order.DateCreated < enddate.Value);
            }

            var orders = await ordersQuery.ToListAsync();

            return orders;
        }


        public bool UpdateOrderStatus(Order order)
        {
            var existingOrder = _context.Orders.Find(order.OrderId);
            if (existingOrder != null)
            {
                existingOrder.Status = order.Status;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

    }

}

   
