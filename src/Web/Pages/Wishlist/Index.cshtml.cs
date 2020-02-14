using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Web.Pages.Basket;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishlistAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Services;

namespace Microsoft.eShopWeb.Web.Pages.Wishlist
{
    public class IndexModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IWishlistService _wishlistService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private string _username = null;
        private readonly IWishlistViewModelService _wishlistViewModelService;

        private readonly IAsyncRepository<WishlistItem> _wishListItemsRepo;
        private readonly IAsyncRepository<CatalogItem> _catalogItemsRepo;

        private readonly IAppLogger<WishlistService> _logger;

        public BasketViewModel BasketModel { get; set; } = new BasketViewModel();


        public IndexModel(IWishlistService wishlistService,
        IAsyncRepository<WishlistItem> wishListItemsRepo,
        IAsyncRepository<CatalogItem> catalogItemsRepo,
            IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            IWishlistViewModelService wishlistViewModelService,
            SignInManager<ApplicationUser> signInManager)
        {
            _wishlistService = wishlistService;
            _signInManager = signInManager;
            _wishlistViewModelService = wishlistViewModelService;
            _basketService = basketService;
            _basketViewModelService = basketViewModelService;
            _wishListItemsRepo = wishListItemsRepo;
            _catalogItemsRepo = catalogItemsRepo;
        }

        public WishlistViewModel WishlistModel { get; set; } = new WishlistViewModel();

        public async Task OnGet()
        {
            await SetWishlistModelAsync();
        }

        public async Task<IActionResult> OnPost(CatalogItemViewModel productDetails)
        {
            if (productDetails?.Id == null)
            {
                return RedirectToPage("/Index");
            }
            await SetWishlistModelAsync();

            await _wishlistService.AddItemToWishlist(WishlistModel.Id, productDetails.Id, productDetails.Price);

            await SetWishlistModelAsync();

            return RedirectToPage();
        }

        public async Task OnPostUpdate(Dictionary<string, int> items)
        {
            await SetWishlistModelAsync();
            await _wishlistService.SetQuantities(WishlistModel.Id, items);

            await SetWishlistModelAsync();
        }

        public async Task<IActionResult> OnPostTransferBasket(Dictionary<string, int> items)
        {
            await SetBasketModelAsync();

            foreach (var kvp in items) {
                var wishListItemId = int.Parse(kvp.Key);
                var wishlistItem = await _wishListItemsRepo.GetByIdAsync(wishListItemId);
                if (wishlistItem == null) {
                     _logger.LogInformation($"No item found in the catalog.");
                
                }
                // wishlistItem.CatalogItemId
                CatalogItem catalogItem = await _catalogItemsRepo.GetByIdAsync(wishlistItem.CatalogItemId);
                await _basketService.AddItemToBasket(BasketModel.Id, wishlistItem.CatalogItemId, catalogItem.Price, kvp.Value);
            }
                       
            return RedirectToPage("/Basket/Index");
        }

       private async Task SetBasketModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetBasketCookieAndUserName();
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            }
        }

        private void GetOrSetBasketCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                _username = Request.Cookies[Constants.BASKET_COOKIENAME];
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
        }


        private async Task SetWishlistModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                WishlistModel = await _wishlistViewModelService.GetOrCreateWishlistForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetWishlistCookieAndUserName();
                WishlistModel = await _wishlistViewModelService.GetOrCreateWishlistForUser(_username);
            }
        }

        private void GetOrSetWishlistCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.WISHLIST_COOKIENAME))
            {
                _username = Request.Cookies[Constants.WISHLIST_COOKIENAME];
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true };
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.WISHLIST_COOKIENAME, _username, cookieOptions);
        }
    }
}