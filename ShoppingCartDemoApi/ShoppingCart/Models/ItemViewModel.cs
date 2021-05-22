using Microsoft.AspNetCore.Http;

namespace ShoppingCart.Models
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public IFormFile Image { get; set; }
    }
}
