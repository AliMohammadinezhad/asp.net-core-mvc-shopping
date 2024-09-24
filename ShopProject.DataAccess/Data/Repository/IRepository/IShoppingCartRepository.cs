using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    void Update(ShoppingCart obj);
}