﻿using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public OrderService(IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
        }

        public async Task CreateOrderAsync(int basketId, Address shippingAddress)
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

            int currentSecond = DateTime.Now.Second;
            if (currentSecond % 3 == 1)
            {
                order.SetStatusOutForDelivery();
            }
            if (currentSecond % 3 == 2)
            {
                order.SetStatusDelivered();
            }

            await _orderRepository.AddAsync(order);
        }
    }
}
