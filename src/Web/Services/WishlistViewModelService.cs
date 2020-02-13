using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishlistAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Pages.Wishlist;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Services
{
    public class WishlistViewModelService : IWishlistViewModelService
    {
        private readonly IAsyncRepository<Wishlist> _wishlistRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public WishlistViewModelService(IAsyncRepository<Wishlist> wishlistRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer)
        {
            _wishlistRepository = wishlistRepository;
            _uriComposer = uriComposer;
            _itemRepository = itemRepository;
        }

        public async Task<WishlistViewModel> GetOrCreateWishlistForUser(string userName)
        {
            var wishlistSpec = new WishlistWithItemsSpecification(userName);
            var wishlist = (await _wishlistRepository.ListAsync(wishlistSpec)).FirstOrDefault();

            if (wishlist == null)
            {
                return await CreateWishlistForUser(userName);
            }
            return await CreateViewModelFromWishlist(wishlist);
        }

        private async Task<WishlistViewModel> CreateViewModelFromWishlist(Wishlist wishlist)
        {
            var viewModel = new WishlistViewModel();
            viewModel.Id = wishlist.Id;
            viewModel.BuyerId = wishlist.BuyerId;
            viewModel.Items = await GetWishlistItems(wishlist.Items); ;
            return viewModel;
        }

        private async Task<WishlistViewModel> CreateWishlistForUser(string userId)
        {
            var wishlist = new Wishlist() { BuyerId = userId };
            await _wishlistRepository.AddAsync(wishlist);

            return new WishlistViewModel()
            {
                BuyerId = wishlist.BuyerId,
                Id = wishlist.Id,
                Items = new List<WishlistItemViewModel>()
            };
        }

        private async Task<List<WishlistItemViewModel>> GetWishlistItems(IReadOnlyCollection<WishlistItem> wishlistItems)
        {
            var items = new List<WishlistItemViewModel>();
            foreach (var item in wishlistItems)
            {
                var itemModel = new WishlistItemViewModel
                {
                    Id = item.Id,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    CatalogItemId = item.CatalogItemId
                };
                var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                itemModel.PictureUrl = _uriComposer.ComposePicUri(catalogItem.PictureUri);
                itemModel.ProductName = catalogItem.Name;
                items.Add(itemModel);
            }

            return items;
        }
    }
}