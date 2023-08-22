//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;

//namespace BookStore.Pages.Shared.Components
//{
//    public class LogoutViewComponent :ViewComponent
//    {
//        public IViewComponentResult Invoke()
//        {
//            if (HttpContext.Session.GetInt32("UserId") != null)
//            {
//                return View();
//            }

//            return Content(""); // Return an empty result if the session is not set
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace BookStore.Pages.Shared.Components
{
    [Controller]
    public class LogoutViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return View();
            }
            if (HttpContext.Session.GetInt32("AdminId") != null)
            {
                return View();
            }

            return Content(""); // Return an empty result if the session is not set
        }
        [HttpPost]
        public IActionResult OnPost()
        {
            _httpContextAccessor.HttpContext.Session.Clear(); // Clear the session

            return new RedirectToPageResult("/Index"); // Redirect to the Login page after logout
                                                             
        }
    }
}

