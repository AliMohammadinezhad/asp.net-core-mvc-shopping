using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopProject.DataAccess.Data;
using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;
using ShopProject.Utility;

namespace MVCProject.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        List<ApplicationUser> objApplicationUsers = _context.ApplicationUsers.Include(u => u.Company).ToList();
        var userRoles = _context.UserRoles.ToList();
        var roles = _context.Roles.ToList();

        foreach (ApplicationUser user in objApplicationUsers)
        {
            var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            user.Role = roles.FirstOrDefault(u => u.Id == roleId)?.Name;

            user.Company ??= new Company { Name = "" };
        }
        
        return Json(new { data = objApplicationUsers });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody]string id)
    {
        var objFromDatabase = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        if (objFromDatabase == null)
        {
            return Json(new { success = true, message = "Error While Locking/Unlocking" });
        }

        if (objFromDatabase.LockoutEnd != null && objFromDatabase.LockoutEnd > DateTime.Now)
        {
            // user is currently locked and we want to unlock them
            objFromDatabase.LockoutEnd = DateTime.Now;
        }
        else
        {
            objFromDatabase.LockoutEnd = DateTime.Now.AddYears(1000);
        }

        _context.SaveChanges();
        return Json(new { success = true, message = "Operation Successful" });
    }

    #endregion
}