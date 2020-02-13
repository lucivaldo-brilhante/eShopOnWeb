using Microsoft.eShopWeb.Web.Pages.Wishlist;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Interfaces
{
    public interface IWishlistViewModelService
    {
        Task<WishlistViewModel> GetOrCreateWishlistForUser(string userName);
    }
}