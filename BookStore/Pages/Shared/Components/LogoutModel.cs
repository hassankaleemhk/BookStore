using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
namespace BookStore.Pages.Shared.Components
{
    public class LogoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult OnPost()
        {
            _httpContextAccessor.HttpContext.Session.Clear(); // Clear the session

            return RedirectToPage("/Users/Index"); // Redirect to the Index page after logout
        }
    }
}
