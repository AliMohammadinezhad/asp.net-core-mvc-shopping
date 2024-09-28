using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace MVCProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API Calls

        public IActionResult GetAll()
        {
            List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            return Json(new { data = objOrderHeaders });
        }

        #endregion

    }
}
