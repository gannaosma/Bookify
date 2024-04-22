namespace Bookify.web.Core.ViewModels
{
    public class CreateCategoryVM
    {
        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
