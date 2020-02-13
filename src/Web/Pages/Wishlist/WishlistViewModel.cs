using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopWeb.Web.Pages.Wishlist
{
    public class WishlistViewModel
    {
        public int Id { get; set; }
        public List<WishlistItemViewModel> Items { get; set; } = new List<WishlistItemViewModel>();
        public string BuyerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
        }
    }
}