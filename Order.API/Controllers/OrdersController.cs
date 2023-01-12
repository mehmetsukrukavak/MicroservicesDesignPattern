using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTOs;
using Order.API.Models;
using Shared;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndPoint;

        public OrdersController(AppDbContext context, IPublishEndpoint publishEndPoint)
        {
            _context = context;
            _publishEndPoint = publishEndPoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreate)
        {
            var newOrder = new Models.Order
            {
                BuyerId = orderCreate.BuyerId,
                OrderStatus = OrderStatus.Suspend,
                Address = new Address
                {
                    Line = orderCreate.Address.Line,
                    District = orderCreate.Address.District,
                    Provience = orderCreate.Address.Provience
                },
                CreatedDate = DateTime.Now
            };


            orderCreate.orderItems.ForEach(item =>
            {
                newOrder.Items.Add(new OrderItem
                {
                    Price = item.Price,
                    Count = item.Count,
                    ProductId = item.ProductId
                });
            });

            await _context.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent
            {
                BuyerId = orderCreate.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage
                {
                    CardName = orderCreate.payment.CardName,
                    CardNumber = orderCreate.payment.CardNumber,
                    CVV = orderCreate.payment.CVV,
                    Expiration = orderCreate.payment.Expiration,
                    TotalPrice = orderCreate.orderItems.Sum(o => o.Price * o.Count)
                }
                
            };

            orderCreate.orderItems.ForEach(item =>
            {
                orderCreatedEvent.orderItems.Add(new OrderItemMessage
                {
                    
                    Count = item.Count,
                    ProductId = item.ProductId
                });
            });
            await _publishEndPoint.Publish(orderCreatedEvent);
            return Ok();
        }
    }
}
