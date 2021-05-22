namespace ShoppingCart.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string ImageName { get; set; }
    }
}
