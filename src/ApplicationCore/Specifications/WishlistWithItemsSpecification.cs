using Microsoft.eShopWeb.ApplicationCore.Entities.WishlistAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications
{
    public class WishlistWithItemsSpecification : BaseSpecification<Wishlist>
    {
         public WishlistWithItemsSpecification(int wishlistId)
            :base(b => b.Id == wishlistId)
        {
            AddInclude(b => b.Items);
        }
        public WishlistWithItemsSpecification(string buyerId)
            :base(b => b.BuyerId == buyerId)
        {
            AddInclude(b => b.Items);
        }
    }
    }