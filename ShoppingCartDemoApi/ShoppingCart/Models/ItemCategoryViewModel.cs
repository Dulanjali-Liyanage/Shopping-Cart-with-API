using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShoppingCart.Models
{
    public class ItemCategoryViewModel
    {
        public List<Item> Items { get; set; }
        public SelectList Category { get; set; }
        public string ItemCategory { get; set; }
        public string SearchString { get; set; }
    }
}
