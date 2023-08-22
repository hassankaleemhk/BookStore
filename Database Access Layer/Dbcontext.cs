using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Access_Layer
{
    public class Dbcontext : DbContext
    {
        public Dbcontext() { }
        public Dbcontext(DbContextOptions<Dbcontext> options) : base(options)
        {
            TestDatabaseConnection();
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }



        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    options.UseSqlServer("Server=DESKTOP-O2KRVQ9\\MSSQLSERVER01;Database=BooksStore;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;");

        //}
        private void TestDatabaseConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection to the database successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection to the database failed: {ex.Message}");
            }
        }

    }
}
