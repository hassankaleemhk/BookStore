using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace BookStore.ViewComponents
{
    public class LogoutViewComponents : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return View();
            }

            return Content(""); // Return an empty result if the session is not set
        }
    }
}
