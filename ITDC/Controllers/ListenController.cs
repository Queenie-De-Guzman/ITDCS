using Microsoft.AspNetCore.Mvc;

namespace ITDC.Controllers
{
    public class ListenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
