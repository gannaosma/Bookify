using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }

		[HttpPost]
        public IActionResult Create(CreateCategoryVM categoryVM)
        {
			if(!ModelState.IsValid)
				return View(categoryVM);

			var catergory = new Category { Name = categoryVM.Name };

			_context.Add(catergory);
			_context.SaveChanges();
			
            return RedirectToAction(nameof(Index));
        }
    }
}
