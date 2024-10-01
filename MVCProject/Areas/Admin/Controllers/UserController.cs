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

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}