using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category obj);
    void Save();
}