using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IWishlistService
    {
        Task<int> GetWishlistItemCountAsync(string userName);
        Task TransferWishlistAsync(string anonymousId, string userName);
        Task TransferWishlistItemToAsync(int wishlistItemId, int basketId, int quantity);
         //{
            // _logger.LogInformation()
            // Get wish list item by id
            // get basket with ther given id
            // get catalog item associated with wishlist item 
            // create new basket item to add to baset
        //}
        
        Task AddItemToWishlist(int wishlistId, int catalogItemId, decimal price, int quantity = 1);
        Task SetQuantities(int wishlistId, Dictionary<string, int> quantities);
        Task DeleteWishlistAsync(int wishlistId);
    }
}