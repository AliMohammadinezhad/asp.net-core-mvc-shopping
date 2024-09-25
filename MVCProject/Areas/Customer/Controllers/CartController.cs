using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;
using ShopProject.Models.ViewModels;
using System.Security.Claims;

namespace MVCProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    //[Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: nameof(Product))
            };
            foreach (ShoppingCart cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            return shoppingCart.Count switch
            {
                <= 50 => shoppingCart.Product.Price,
                > 50 and <= 100 => shoppingCart.Product.Price50,
                > 100 => shoppingCart.Product.Price100
            };
        }
    }
}