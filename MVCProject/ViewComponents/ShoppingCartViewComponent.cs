using Microsoft.AspNetCore.Mvc;
using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Utility;
using System.Security.Claims;

namespace MVCProject.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        Claim? claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (claim?.Value != null)
        {
            if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).Count());
            }

            return View(HttpContext.Session.GetInt32(SD.SessionCart));
        }
        else
        {
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}