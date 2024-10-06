using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _context;
    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(ApplicationUser applicationUser)
    {
        _context.ApplicationUsers.Update(applicationUser);
    }
}