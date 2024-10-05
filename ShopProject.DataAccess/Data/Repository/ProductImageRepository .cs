using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository;

public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly ApplicationDbContext _context;
    public ProductImageRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(ProductImage obj)
    {
        _context.ProductImages.Update(obj);
    }

    
}