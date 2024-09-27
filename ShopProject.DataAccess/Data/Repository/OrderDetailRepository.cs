using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _context;
    public OrderDetailRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(OrderDetail obj)
    {
        _context.OrderDetails.Update(obj);
    }

    
}