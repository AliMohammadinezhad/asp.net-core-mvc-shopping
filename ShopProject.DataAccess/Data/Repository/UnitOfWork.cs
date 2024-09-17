using ShopProject.DataAccess.Data.Repository.IRepository;

namespace ShopProject.DataAccess.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Category = new CategoryRepository(_context);
        Product = new ProductRepository(_context);
    }


    public void Save()
    {
        _context.SaveChanges();
    }
}