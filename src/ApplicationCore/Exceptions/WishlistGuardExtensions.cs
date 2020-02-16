using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishlistAggregate;

namespace ApplicationCore.Exceptions
{
    public static class WishlistGuardExtensions
    {
        public static void NullWishlist(this IGuardClause guardClause, int wishlistId, Wishlist wishlist)
        {
            if (wishlist == null)
                throw new WishlistNotFoundException(wishlistId);
        }
    }
}