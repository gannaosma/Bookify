﻿namespace Bookify.web.Core.Models
{
    [Index(nameof(Title), nameof(AuthorId), IsUnique = true)]
    public class Book: BaseModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = null!;

        public int AuthorId { get; set; }

        public Author? Author { get; set; }

        [MaxLength(200)]
        public string Publisher { get; set; } = null!;

        public DateTime PublishingDate { get; set; }

        public string? Image { get; set; }

        [MaxLength(50)]
        public string Hall { get; set; } = null!;

        public bool IsAvailabeForRental { get; set; }
        
        public string Description { get; set; } = null!;

        public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();
    }
}
