namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    void Save();
}