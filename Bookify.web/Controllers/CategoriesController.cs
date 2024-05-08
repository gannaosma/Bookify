using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.web.Controllers
{
	public class CategoriesController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CategoriesController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			// TODO use ViewModel
			var categories = _context.Categories.ToList();
			return View(categories);
		}

		[HttpGet]
        public IActionResult Create()
        {
            return View("Form");
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryFormVM categoryVM)
        {
			if(!ModelState.IsValid)
				return View("Form",categoryVM);

			var catergory = new Category { Name = categoryVM.Name };

			_context.Add(catergory);
			_context.SaveChanges();
			
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);

            if (category is null)
                return NotFound();

            var categoryVM = new CategoryFormVM
            {
                Id = category.Id,
                Name = category.Name 
            };
            return View("Form",categoryVM);

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryFormVM categoryVM)
        {
            if (!ModelState.IsValid)
                return View(categoryVM);

            var category = _context.Categories.Find(categoryVM.Id);

            if (category is null)
                return NotFound();


            category.Name = categoryVM.Name;
            category.LastUpdatedOn = DateTime.Now;
            

            _context.Update(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
