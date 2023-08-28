using Amazon.Lambda.Core;
using Database_Access_Layer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaProcessPendingStatus;

public class Function
{
    //private readonly Dbcontext _dbcontext;
    //public Function(Dbcontext dbcontext)
    //{
    //    _dbcontext = dbcontext;
    //}
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler()
    {
        var connectionString = "Server=database-1.cotmfcems9t1.us-east-1.rds.amazonaws.com,1433;DataBase=BookStore1;User ID=admin;Password=mypassword;TrustServerCertificate=true;";
        var optionsBuilder = new DbContextOptionsBuilder<Dbcontext>().UseSqlServer(connectionString);

        var _dbcontext = new Dbcontext(optionsBuilder.Options);

        var orders = await _dbcontext.Orders.Where(or => or.Status == "Pending").ToListAsync();
        if(orders != null)
        {
            foreach(var order in orders)
            {
                StatusUpdate(order.OrderId,_dbcontext);
            }
        }
        else
        {
            Console.WriteLine($"No Orders in Pending Status");
        }
    }
    public void StatusUpdate(int OrderId, Dbcontext dbContext)
    {
        var existingOrder = dbContext.Orders.Find(OrderId);
        if (existingOrder != null)
        {
            existingOrder.Status = "Processed";
            dbContext.SaveChanges();
        }
    }
}
