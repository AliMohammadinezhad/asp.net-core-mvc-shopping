using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Product obj)
    {
        _context.Products.Update(obj);
    }
}