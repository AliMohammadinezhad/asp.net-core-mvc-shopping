using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _context;
    public OrderHeaderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(OrderHeader obj)
    {
        _context.OrderHeaders.Update(obj);
    }

}