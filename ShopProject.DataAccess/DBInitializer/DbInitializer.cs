using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopProject.DataAccess.Data;
using ShopProject.Models;
using ShopProject.Utility;

namespace ShopProject.DataAccess.DBInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public DbInitializer(
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager
        )
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public void Initialize()
    {
        try
        {
            // migrations if they are not applied
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        // create roles if they are not created
        if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

            // if roles are not created, then we will admin user as well
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                Name = "admin",
                PhoneNumber = "09111111111",
                StreetAddress = "test ave.",
                State = "NY",
                PostalCode = "23422",
                City = "NewYork"
            }, "@Admin123").GetAwaiter().GetResult();

            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@admin.com");
            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }

        return;
    }
}