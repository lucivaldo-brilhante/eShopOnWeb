using ApplicationCore.Exceptions;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishlistAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IAsyncRepository<Wishlist> _wishlistRepository;
        private readonly IAppLogger<WishlistService> _logger;

        public WishlistService(IAsyncRepository<Wishlist> wishlistRepository,
            IAppLogger<WishlistService> logger)
        {
            _wishlistRepository = wishlistRepository;
            _logger = logger;
        }

        public async Task AddItemToWishlist(int wishlistId, int catalogItemId, decimal price, int quantity = 1)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(wishlistId);

            wishlist.AddItem(catalogItemId, price, quantity);

            await _wishlistRepository.UpdateAsync(wishlist);
        }

        public async Task DeleteWishlistAsync(int wishlistId)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(wishlistId);
            await _wishlistRepository.DeleteAsync(wishlist);
        }

        public async Task<int> GetWishlistItemCountAsync(string userName)
        {
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            var wishlistSpec = new WishlistWithItemsSpecification(userName);
            var wishlist = (await _wishlistRepository.ListAsync(wishlistSpec)).FirstOrDefault();
            if (wishlist == null)
            {
                _logger.LogInformation($"No wish list found for {userName}");
                return 0;
            }
            int count = wishlist.Items.Sum(i => i.Quantity);
            _logger.LogInformation($"Wish list for {userName} has {count} items.");
            return count;
        }

        public async Task SetQuantities(int wishlistId, Dictionary<string, int> quantities)
        {
            Guard.Against.Null(quantities, nameof(quantities));
            var wishlist = await _wishlistRepository.GetByIdAsync(wishlistId);
            Guard.Against.NullWishlist(wishlistId, wishlist);
            foreach (var item in wishlist.Items)
            {
                if (quantities.TryGetValue(item.Id.ToString(), out var quantity))
                {
                    if (_logger != null) _logger.LogInformation($"Updating quantity of item ID:{item.Id} to {quantity}.");
                    item.Quantity = quantity;
                }
            }
            wishlist.RemoveEmptyItems();
            await _wishlistRepository.UpdateAsync(wishlist);
        }

        public async Task TransferWishlistAsync(string anonymousId, string userName)
        {
            Guard.Against.NullOrEmpty(anonymousId, nameof(anonymousId));
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            var wishlistSpec = new WishlistWithItemsSpecification(anonymousId);
            var wishlist = (await _wishlistRepository.ListAsync(wishlistSpec)).FirstOrDefault();
            if (wishlist == null) return;
            wishlist.BuyerId = userName;
            await _wishlistRepository.UpdateAsync(wishlist);
        }
    }
}