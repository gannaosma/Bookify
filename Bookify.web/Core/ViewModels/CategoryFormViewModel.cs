namespace Bookify.web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "Max Length can't be more than 100 charcter")]
        [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = "Category with the Same name is already exist")]
        public string Name { get; set; } = null!;
    }
}
