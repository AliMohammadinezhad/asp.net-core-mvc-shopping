using ShopProject.DataAccess.Data.Repository.IRepository;

namespace ShopProject.DataAccess.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICompanyRepository Company { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; set; }
    public IOrderHeaderRepository OrderHeader { get; private set; }
    public IOrderDetailRepository OrderDetail { get; private set; }
    public IProductImageRepository ProductImage { get; private set; }
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Category = new CategoryRepository(_context);
        Product = new ProductRepository(_context);
        Company = new CompanyRepository(_context);
        ShoppingCart = new ShoppingCartRepository(_context);
        ApplicationUser = new ApplicationUserRepository(_context);
        OrderHeader = new OrderHeaderRepository(_context);
        OrderDetail = new OrderDetailRepository(_context);
        ProductImage = new ProductImageRepository(_context);
    }


    public void Save()
    {
        _context.SaveChanges();
    }
}