using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    public class AuthConstoller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
