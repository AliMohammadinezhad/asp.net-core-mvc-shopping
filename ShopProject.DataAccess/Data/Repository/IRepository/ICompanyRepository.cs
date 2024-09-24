using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    void Update(Company obj);
}