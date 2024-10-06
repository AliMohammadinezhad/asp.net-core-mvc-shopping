using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    public void Update(ApplicationUser applicationUser);
}