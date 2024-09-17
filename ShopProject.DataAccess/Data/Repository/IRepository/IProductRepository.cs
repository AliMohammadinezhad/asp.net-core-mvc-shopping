using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product obj);
}