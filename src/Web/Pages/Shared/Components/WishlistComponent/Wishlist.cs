using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Pages.Wishlist;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Pages.Shared.Components.WishlistComponent
{
    public class Wishlist : ViewComponent
    {
        private readonly IWishlistViewModelService _wishlistService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Wishlist(IWishlistViewModelService wishlistService,
                        SignInManager<ApplicationUser> signInManager)
        {
            _wishlistService = wishlistService;
            _signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userName)
        {
            var vm = new WishlistComponentViewModel();
            vm.ItemsCount = (await GetWishlistViewModelAsync()).Items.Sum(i => i.Quantity);
            return View(vm);
        }

        private async Task<WishlistViewModel> GetWishlistViewModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return await _wishlistService.GetOrCreateWishlistForUser(User.Identity.Name);
            }
            string anonymousId = GetWishlistIdFromCookie();
            if (anonymousId == null) return new WishlistViewModel();
            return await _wishlistService.GetOrCreateWishlistForUser(anonymousId);
        }

        private string GetWishlistIdFromCookie()
        {
            if (Request.Cookies.ContainsKey(Constants.WISHLIST_COOKIENAME))
            {
                return Request.Cookies[Constants.WISHLIST_COOKIENAME];
            }
            return null;
        }
    }
}