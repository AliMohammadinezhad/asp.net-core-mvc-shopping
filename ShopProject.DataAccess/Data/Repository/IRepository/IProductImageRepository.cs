using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IProductImageRepository : IRepository<ProductImage>
{
    void Update(ProductImage obj);
}