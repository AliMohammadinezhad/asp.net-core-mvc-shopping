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
        var objectFromDb = _context.Products.FirstOrDefault(u => u.Id == obj.Id);
        if (objectFromDb != null)
        {
            objectFromDb.Title = obj.Title;
            objectFromDb.ISBN = obj.ISBN;
            objectFromDb.Price = obj.Price;
            objectFromDb.Price50 = obj.Price50;
            objectFromDb.Price100 = obj.Price100;
            objectFromDb.ListPrice = obj.ListPrice;
            objectFromDb.Description = obj.Description;
            objectFromDb.CategoryId = obj.CategoryId;
            objectFromDb.Author = obj.Author;
            objectFromDb.ProductImages = obj.ProductImages;
        }
    }
}