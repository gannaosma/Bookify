namespace Bookify.web.Core.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public DateTime PublishingDate { get; set; }

        public string? Image { get; set; }

        public string Hall { get; set; } = null!;

        public bool IsAvailabeForRental { get; set; }

        public string Description { get; set; } = null!;

        public IEnumerable<string> Categories { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } 
    }
}
