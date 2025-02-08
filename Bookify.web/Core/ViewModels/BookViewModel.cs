using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.web.Core.ViewModels
{
    public class BookViewModel
    {

        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLength)]
        public string Title { get; set; } = null!;

        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        public IEnumerable<SelectListItem>? Authors { get; set; }

        [MaxLength(200, ErrorMessage = Errors.MaxLength)]
        public string Publisher { get; set; } = null!;

        [Display(Name = "Publishing Date")]
        public DateTime PublishingDate { get; set; }

        public IFormFile? Image { get; set; }

        [MaxLength(50, ErrorMessage = Errors.MaxLength)]
        public string Hall { get; set; } = null!;

        [Display(Name = "Is availabe for rental?")]
        public bool IsAvailabeForRental { get; set; }

        public string Description { get; set; } = null!;

        [Display(Name = "Categories")]
        public IList<int> SelectedCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }

    }
}
