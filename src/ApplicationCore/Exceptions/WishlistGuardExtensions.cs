using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishlistAggregate;
using Ardalis.GuardClauses;

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