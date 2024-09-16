using Microsoft.AspNetCore.Mvc;
using MVCProject.Data;
using MVCProject.Models;

namespace MVCProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            return View(categories);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "the Display Order Cannot exactly match the name");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Add(obj);
                _context.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
            
        }
    }
}
