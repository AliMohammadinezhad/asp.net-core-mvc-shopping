using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopProject.DataAccess.Data.Repository;
using ShopProject.Models;
using ShopProject.Models.ViewModels;
using ShopProject.Utility;

namespace MVCProject.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly UnitOfWork _unitOfWork;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(
        UnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RoleManagement(string userId)
    {
        RoleManagementVM RoleVM = new()
        {
            ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
            RoleList = _roleManager.Roles.Select(i => new SelectListItem { Text = i.Name, Value = i.Name }),
            CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name, Value = i.Id.ToString()
            })
        };

        // Ensure ApplicationUser is not null before accessing properties
        if (RoleVM.ApplicationUser != null)
        {
            // Get the role name only if roleId is not null
            var role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId)).GetAwaiter()
                .GetResult();
            RoleVM.ApplicationUser.Role = role?.FirstOrDefault(); // Use null-conditional operator
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
        if (roleManagementVm?.ApplicationUser == null)
        {
            // Handle the error (e.g., return a BadRequest or similar response)
            return BadRequest("Invalid role management data.");
        }

        string? oldRole = _userManager
            .GetRolesAsync(_unitOfWork.ApplicationUser
                .Get(u => u.Id == roleManagementVm.ApplicationUser.Id)
            )
            .GetAwaiter()
            .GetResult()
            .FirstOrDefault();

        if (oldRole == null)
        {
            // Handle the case where the role is not found
            return NotFound("Role not found.");
        }

        ApplicationUser applicationUser =
            _unitOfWork.ApplicationUser
                .Get(u => u.Id == roleManagementVm.ApplicationUser.Id);

        if (roleManagementVm.ApplicationUser.Role != oldRole)
        {
            // a role was updated
           
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
            _unitOfWork.ApplicationUser.Update(applicationUser);
            _unitOfWork.Save();

            _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(applicationUser, roleManagementVm.ApplicationUser.Role).GetAwaiter()
                .GetResult();
        }
        else if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagementVm.ApplicationUser.CompanyId)
        {
            applicationUser.CompanyId = roleManagementVm.ApplicationUser.CompanyId;
            _unitOfWork.ApplicationUser.Update(applicationUser);
            _unitOfWork.Save();
        }


        return RedirectToAction(nameof(Index));
    }


    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        List<ApplicationUser> objApplicationUsers =
            _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

        foreach (ApplicationUser user in objApplicationUsers)
        {
            user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            user.Company ??= new Company() { Name = "" };
        }

        return Json(new { data = objApplicationUsers });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody] string id)
    {
        var objFromDatabase = _unitOfWork.ApplicationUser
            .Get(u => u.Id == id);
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
        _unitOfWork.ApplicationUser.Update(objFromDatabase);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Operation Successful" });
    }

    #endregion
}