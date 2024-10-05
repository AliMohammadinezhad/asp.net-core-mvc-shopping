using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;
using ShopProject.Models.ViewModels;
using ShopProject.Utility;

namespace MVCProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            
            return View(products);
        }


        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            
            ProductVM productVm = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVm);
            }
            else
            {
                //update
                productVm.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVm);
            }

            

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "product added successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "product updated successfully";
                }

                _unitOfWork.Save();

                string wwwrootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + obj.Product.Id;
                        string finalPath = Path.Combine(wwwrootPath, productPath);
                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = obj.Product.Id
                        };

                        if (obj.Product.ProductImages == null)
                            obj.Product.ProductImages = new List<ProductImage>();

                        obj.Product.ProductImages.Add(productImage);

                    }
                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();



                    
                }

                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
                return View(obj);
            }
            

        }

        #region API Calls

        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == id);
            
            if (product == null)
                return Json(new { success = false, message = "error while deleting" });

            //string wwwrootPath = _webHostEnvironment.WebRootPath;
            //var oldImagePath = Path.Combine(wwwrootPath, product.ImageUrl.TrimStart('\\'));
            //if (System.IO.File.Exists(oldImagePath))
            //    System.IO.File.Delete(oldImagePath);


            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product Deleted Successfully"});
        }

        #endregion
    }
}
