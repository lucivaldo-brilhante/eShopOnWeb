using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;

namespace Microsoft.eShopWeb.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IHtmlLocalizer<HomeController> _localizer;

        public HomeController(IHtmlLocalizer<HomeController> localizer)
        {
            this._localizer = localizer;
        }



        public IActionResult Privacy()
        {
            ViewData["Message"] = _localizer["Politic Privacy blalalal."];

            return View();
        }

        public IActionResult SetCulture(string id = "en-US")
        {
            string culture = id;
            Response.Cookies.Append(
               CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
               new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
           );

            ViewData["Message"] = "Culture set to " + culture;

            return View("Privacy");
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult Error()
        {
            return View();
        }


    }
}