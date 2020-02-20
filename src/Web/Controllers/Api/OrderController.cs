using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.Web.Features.MyOrders;
using Microsoft.eShopWeb.Web.Features.OrderDetails;
using System;
using System.Threading.Tasks;
using IronPdf;
using System.Net;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Threading;
using System.Linq;

namespace Microsoft.eShopWeb.Web.Controllers.Api
{
    public class OrderUpdate {
        public string Status { get; set; }
        public string Description { get; set; }
    } 


    public class OrderController : BaseApiController
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailSender _emailSender;
        private readonly  IAppLogger<OrderController> _logger;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public OrderController(
            IAppLogger<OrderController> logger,
            IOrderRepository orderRepository,
            IEmailSender emailSender, IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _emailSender = emailSender;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) 
            {
                _logger.LogWarning($"Order with ID = {order.Id}  not found");
                return NotFound();
            }
            await _orderRepository.DeleteAsync(order);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrderAsync(int basketId, Address shippingAddress)
        { 

            var basket = await _basketRepository.GetByIdAsync(basketId);
            Guard.Against.NullBasket(basketId, basket);
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, catalogItem.PictureUri);
                var orderItem = new OrderItem(itemOrdered, item.UnitPrice, item.Quantity);
                items.Add(orderItem);
            }
            var order = new Order(basket.BuyerId, shippingAddress, items);
            await _orderRepository.AddAsync(order);
            return Ok(order);
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> ShowOrderByUserName(string UserName)
        {
            var specification = new CustomerOrdersWithItemsSpecification(UserName);
            var orders = await _orderRepository.ListAsync(specification);

            return orders.Select(o => new OrderViewModel
            {
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems?.Select(oi => new OrderItemViewModel()
                {
                    PictureUrl = oi.ItemOrdered.PictureUri,
                    ProductId = oi.ItemOrdered.CatalogItemId,
                    ProductName = oi.ItemOrdered.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Units = oi.Units
                }).ToList(),
                OrderNumber = o.Id,
                ShippingAddress = o.ShipToAddress,
                Status = GetDetailedStatus(o.Status),
                Total = o.Total()
            });
        }

        private string GetDetailedStatus(string status)
        {
            if (status == "Pending") return status;
            if (status == "Out for Delivery")
            {
                return $"{status} - ETA {DateTime.Now.AddHours(1).ToShortTimeString()}";
            }
            if (status == "Delivered")
            {
                return $"{status} at {DateTime.Now.AddHours(-1).ToShortTimeString()}";
            }
            return "Unknown";
        }
      
        [HttpPut]
        public async Task<ActionResult<Order>> UpdateById(int orderId, [FromBody] OrderUpdate data)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            _logger.LogInformation($"Getting order with ID = {order.Id} ");
            if (order == null) 
            {
                _logger.LogWarning($"Order with ID = {order.Id}  not found");
                return NotFound();
            }
            var oldStatus = order.Status;
            order.Status = data.Status;
            await _orderRepository.UpdateAsync(order);
            if (!oldStatus.Equals(data.Status, StringComparison.CurrentCultureIgnoreCase)) 
            {
                // Notificar utilizador
                _logger.LogInformation("Sending user notification");
                await _emailSender.SendEmailAsync("useremail", $"Order {order.Id} status changed from {oldStatus} to {data.Status}", "message context");
            }
          
            return Ok(order);
        }

    }

}