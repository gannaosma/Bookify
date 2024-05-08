namespace Bookify.web.Core.ViewModels
{
    public class CategoryFormVM
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
