using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
}