using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopProject.DataAccess.Data;
using ShopProject.Models;
using ShopProject.Models.ViewModels;
using ShopProject.Utility;

namespace MVCProject.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RoleManagement(string userId)
    {
        var userRole = _context.UserRoles.FirstOrDefault(u => u.UserId == userId);
        // Check if the user role exists
        if (userRole == null)
        {
            // Handle the case where the user role is not found (e.g., return a not found response or a default view)
            return NotFound("User role not found.");
        }

        string roleId = userRole.RoleId;


        RoleManagementVM RoleVM = new()
        {
            ApplicationUser = _context.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
            RoleList = _context.Roles.Select(i => new SelectListItem
            {
                Text = i.Name, Value = i.Name
            }),
            CompanyList = _context.Companies.Select(i => new SelectListItem
            {
                Text = i.Name, Value = i.Id.ToString()
            })
        };

        // Ensure ApplicationUser is not null before accessing properties
        if (RoleVM.ApplicationUser != null)
        {
            // Get the role name only if roleId is not null
            var role = _context.Roles.FirstOrDefault(u => u.Id == roleId);
            RoleVM.ApplicationUser.Role = role?.Name; // Use null-conditional operator
        }
        else
        {
            // Handle case where ApplicationUser is not found
            return NotFound("Application user not found.");
        }
        return View(RoleVM);
    }

    [HttpPost]
    public IActionResult RoleManagement(RoleManagementVM roleManagementVm)
    {
        if (roleManagementVm == null || roleManagementVm.ApplicationUser == null)
        {
            // Handle the error (e.g., return a BadRequest or similar response)
            return BadRequest("Invalid role management data.");
        }
        var userRole = _context.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVm.ApplicationUser.Id);
        if (userRole == null)
        {
            // Handle the case where the user role is not found
            return NotFound("User role not found.");
        }

        string roleId = userRole.RoleId;
        var oldRole = _context.Roles.FirstOrDefault(u => u.Id == roleId).Name;
        if (oldRole == null)
        {
            // Handle the case where the role is not found
            return NotFound("Role not found.");
        }

        if (roleManagementVm.ApplicationUser.Role != oldRole)
        {
            // a role was updated
            ApplicationUser applicationUser =
                _context.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVm.ApplicationUser.Id);

            if (applicationUser == null)
            {
                // Handle the case where the application user is not found
                return NotFound("Application user not found.");
            }

            if (roleManagementVm.ApplicationUser.Role == SD.Role_Company)
            {
                applicationUser.CompanyId = roleManagementVm.ApplicationUser.CompanyId;
            }

            else if (oldRole == SD.Role_Company)
            {
                applicationUser.CompanyId = null;
            }
            _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(applicationUser, roleManagementVm.ApplicationUser.Role).GetAwaiter()
                .GetResult();

            _context.SaveChanges();

            
        }
        else
        {
        }


        return RedirectToAction(nameof(Index));
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
            var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
            user.Role = roles.FirstOrDefault(u => u.Id == roleId)?.Name;

            if (user.Company == null)
            {
                user.Company = new Company() { Name = "" };
            }
        }

        return Json(new { data = objApplicationUsers });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody] string id)
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