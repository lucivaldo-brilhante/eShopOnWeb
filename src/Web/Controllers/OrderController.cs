using IronPdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.Web.Features.MyOrders;
using Microsoft.eShopWeb.Web.Features.OrderDetails;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.Extensions.Localization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize] // Controllers that mainly require Authorization still use Controller/View; other pages use Pages
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IViewRenderService _viewRenderService;

        public OrderController(IMediator mediator, IStringLocalizer<OrderController> stringLocalizer, IViewRenderService viewRenderService)
        {
            _mediator = mediator;
            _viewRenderService = viewRenderService;
        }

        [HttpGet()]
        public async Task<IActionResult> MyOrders()
        {
            var viewModel = await _mediator.Send(new GetMyOrders(User.Identity.Name));

            return View(viewModel);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> MyOrdersPdf()
        {
            var viewModel = await _mediator.Send(new GetMyOrders(User.Identity.Name));

            return View(viewModel);
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> Detail(int orderId)
        {
            var viewModel = await _mediator.Send(new GetOrderDetails(User.Identity.Name, orderId));

            if (viewModel == null)
            {
                return BadRequest("No such order found for this user.");
            }

            return View(viewModel);
        }

        [HttpGet("{orderId}/pdf")]
        public async Task<IActionResult> DetailPdf(int orderId)
        {
            var viewModel = await _mediator.Send(new GetOrderDetails(User.Identity.Name, orderId));

            if (viewModel == null)
            {
                return BadRequest("No such order found for this user.");
            }
            var html = await _viewRenderService.RenderToStringAsync("Order/Detail_Doc", viewModel);
            IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
            var pdfDoc = await Renderer.RenderHtmlAsPdfAsync(html);

            return File(pdfDoc.BinaryData, "application/pdf", $"order{orderId}.pdf");

            // var viewResult = await Detail(orderId) as ViewResult;
            // var renderedView = await viewRenderService.RenderToStringAsync(viewResult.ViewName, viewResult.Model);
        }
    }
}
