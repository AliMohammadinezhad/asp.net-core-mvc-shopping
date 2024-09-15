using Microsoft.AspNetCore.Mvc;

namespace MVCProject.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
