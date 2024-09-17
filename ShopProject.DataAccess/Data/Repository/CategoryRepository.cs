using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Category obj)
    {
        _context.Categories.Update(obj);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}