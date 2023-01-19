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
using Shared.Events;
using Shared.Interfaces;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
       


        private readonly ISendEndpointProvider _sendEndpointProvider;
        public OrdersController(AppDbContext context, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
           
            _sendEndpointProvider = sendEndpointProvider;
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

            var orderCreatedRequestEvent = new OrderCreatedRequestEvent
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
                orderCreatedRequestEvent.OrderItems.Add(new OrderItemMessage
                {
                    
                    Count = item.Count,
                    ProductId = item.ProductId
                });
            });
            var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.OrderSaga}"));

            await sendEndPoint.Send<IOrderCreatedRequestEvent>(orderCreatedRequestEvent);
         
            return Ok();
        }
    }
}
