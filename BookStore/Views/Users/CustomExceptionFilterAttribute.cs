using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.Views.Users
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            
            var exception = context.Exception;
            
            context.Result = new JsonResult("An error occurred. Please try again later.")
            {
                StatusCode = 500
            };

           
            context.ExceptionHandled = true;
        }
    }
}
