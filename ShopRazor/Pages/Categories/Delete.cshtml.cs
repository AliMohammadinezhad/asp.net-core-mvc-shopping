using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopRazor.Data;
using ShopRazor.Models;

namespace ShopRazor.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult OnGet(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            Category = _context.Categories.Find(id);
            return Page();
        }

        public IActionResult OnPost()
        {
            Category? obj = _context.Categories.Find(Category.Id);
            
            if (obj == null)
                return NotFound();
            
            _context.Categories.Remove(obj); 
            _context.SaveChanges();
            TempData["error"] = "Category Deleted Successfully";
            return RedirectToPage("index");
            
            
        }
    }
}
