using Microsoft.AspNetCore.Mvc;
using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace MVCProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
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
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
            
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Category? category = _unitOfWork.Category.Get(cat => cat.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "the Display Order Cannot exactly match the name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View(obj);

        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Category? category = _unitOfWork.Category.Get(cat => cat.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Category? category = _unitOfWork.Category.Get(cat => cat.Id == id);
            
            if (category == null)
                return NotFound();
            
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData["error"] = "Category Deleted Successfully";
            return RedirectToAction("Index", "Category");
            

        }

    }
}
