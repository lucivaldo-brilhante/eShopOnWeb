﻿using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.Web.Extensions;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogViewModelService _catalogViewModelService;

        public IndexModel(
            ICatalogViewModelService catalogViewModelService,
            IStringLocalizer<IndexModel> stringLocalizer,
            IHtmlLocalizer<IndexModel> htmlLocalizer
        )
        {
            _catalogViewModelService = catalogViewModelService;
        }

        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        public async Task OnGet(CatalogIndexViewModel catalogModel, int? pageId)
        {
            CatalogModel = await _catalogViewModelService.GetCatalogItems(
                pageId ?? 0, Constants.ITEMS_PER_PAGE,
                catalogModel.SearchText,
                catalogModel.BrandFilterApplied, catalogModel.TypesFilterApplied,
                convertPrice: true,
                HttpContext.RequestAborted);
            CatalogModel.ResultView = catalogModel.ResultView; // HACK
            CatalogModel.ResultViews = Enum<ResultView>.GetAll()
                .Select(resultView => new SelectListItem { Value = resultView.ToString(), Text = resultView.ToString() });
        }
    }
}