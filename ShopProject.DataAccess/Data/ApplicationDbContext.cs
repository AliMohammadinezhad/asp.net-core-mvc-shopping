using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopProject.Models;

namespace ShopProject.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
            new Category { Id = 3, Name = "History", DisplayOrder = 3 }
        );

        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Name = "Tech Solution",
                StreetAddress = "123 Tech St",
                City = "Tech City",
                PostalCode = "123123",
                PhoneNumber = "66669999000",
                State = "IL"
            },
            new Company
            {
                Id = 2,
                Name = "Vivid Books",
                StreetAddress = "123 Vid St",
                City = "Vid City",
                PostalCode = "432432",
                PhoneNumber = "77766444222",
                State = "IL"
            },
            new Company
            {
                Id = 3,
                Name = "Readers Club",
                StreetAddress = "99 Main St",
                City = "Lala Land",
                PostalCode = "99999",
                PhoneNumber = "33349999780",
                State = "NY"
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Author = "Ali",
                Title = "Good Programmer",
                Description = "Book for programming",
                ISBN = "123489109123",
                ListPrice = 103.0,
                Price = 90,
                Price100 = 70,
                Price50 = 80,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Author = "Mohammad",
                Title = "C# in Action",
                Description = "C# World",
                ISBN = "1234BN9109123",
                ListPrice = 200.0,
                Price = 190,
                Price100 = 170,
                Price50 = 180,
                CategoryId = 2
            },
            new Product
            {
                Id = 3,
                Author = "Reza",
                Title = ".NET in action",
                Description = ".NET in real world",
                ISBN = "1234DF23G3",
                ListPrice = .0,
                Price = 70,
                Price100 = 50,
                Price50 = 60,
                CategoryId = 3
            });
    }
}