﻿using Microsoft.AspNetCore.Mvc;
using MVCProject.Data;
using MVCProject.Models;

namespace MVCProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            return View(categories);
        }
    }
}
