using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Dynamic.Core;

namespace Bookify.web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private List<string> _allowedExtensions = new() { ".png", ".jpeg", ".jpg" };
        private int _maxSize = 2097152;

        public BooksController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetBooks()
        {
            var skip = int.Parse(Request.Form["start"]);
            var pageSize = int.Parse(Request.Form["length"]);

            var searchValue = Request.Form["search[value]"];

            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<Book> books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .ThenInclude(b => b.Category);

            if (!searchValue.IsNullOrEmpty())
                books = books.Where(b => b.Title.Contains(searchValue!) || b.Author!.Name.Contains(searchValue!));


            books = books.OrderBy($"{sortColumn} {sortColumnDirection}");

            var data = books.Skip(skip).Take(pageSize).ToList();
            var MappedDate = _mapper.Map<IEnumerable<BookViewModel>>(data);

            var recordsTotal = books.Count();

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = MappedDate };
            return Ok(jsonData);
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .ThenInclude(b => b.Category)
                .SingleOrDefault(b => b.Id == id);

            if (book is null)
                return NotFound();

            var bookVM = _mapper.Map<BookViewModel>(book);

            return View(bookVM);
        }

        public IActionResult Create()
        {
            return View("Form", PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if(!ModelState.IsValid)
                return View("Form",  PopulateViewModel(model));

            var book = _mapper.Map<Book>(model);

            if (model.ImageForm is not null)
            {
                var extension = Path.GetExtension(model.ImageForm.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.ImageForm), Errors.AllowedExtensions);
                    return View("Form", PopulateViewModel(model));
                }

                if(model.ImageForm.Length > _maxSize)
                {
                    ModelState.AddModelError(nameof(model.ImageForm), Errors.MaxSize);
                    return View("Form", PopulateViewModel(model));
                }

                var imageName = $"{Guid.NewGuid()}{extension}";

                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

                //using var stream = System.IO.File.Create(path);
                //model.ImageForm.CopyTo(stream);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageForm.CopyToAsync(stream);
                }

                book.Image = imageName;
            }

            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });
            
            _context.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == id);

            var model = _mapper.Map<BookFormViewModel>(book);
            var viewModel = PopulateViewModel(model);

            viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();

            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == model.Id);

            if (book is null)
                return NotFound();

            if (model.ImageForm is not null)
            {
                if (!string.IsNullOrEmpty(book.Image))
                {
                    var oldPath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", book.Image);

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var extension = Path.GetExtension(model.ImageForm.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.ImageForm), Errors.AllowedExtensions);
                    return View("Form", PopulateViewModel(model));
                }

                if (model.ImageForm.Length > _maxSize)
                {
                    ModelState.AddModelError(nameof(model.ImageForm), Errors.MaxSize);
                    return View("Form", PopulateViewModel(model));
                }

                var imageName = $"{Guid.NewGuid()}{extension}";

                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageForm.CopyToAsync(stream);
                }
                book.Image = imageName;
            }

            else if (!string.IsNullOrEmpty(model.ImageName))
                book.Image = model.ImageName;

            book = _mapper.Map(model, book);
            book.LastUpdatedOn = DateTime.Now;

            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });

            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var book = _context.Books.Find(id);

            if (book is null)
                return NotFound();

            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();

            return Ok();
        }

        public IActionResult AllowItem(BookFormViewModel model)
        {
            var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId == model.AuthorId);
            var isAllowed = book is null || book.Id.Equals(model.Id);
            return Json(isAllowed);
        }


        private BookFormViewModel PopulateViewModel(BookFormViewModel? model = null)
        {
            var viewModel = (model is null) ? new BookFormViewModel() : model;

            var authors = _context.Authors.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
            var categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList();

            viewModel.Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);

            return viewModel;
        }
    }
}
