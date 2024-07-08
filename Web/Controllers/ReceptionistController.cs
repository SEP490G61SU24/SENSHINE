using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ReceptionistController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
