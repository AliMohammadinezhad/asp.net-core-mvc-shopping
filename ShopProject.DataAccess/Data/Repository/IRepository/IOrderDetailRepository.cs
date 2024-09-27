using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
}