﻿using System;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Interfaces;
using Stock.API.Models;

namespace Stock.API.Consumers
{
	public class OrderCreatedEventConsumer:IConsumer<IOrderCreatedEvent>
	{
        private readonly AppDbContext _context;
        private ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext context,
                                         ILogger<OrderCreatedEventConsumer> logger,
                                         ISendEndpointProvider sendEndpointProvider,
                                         IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            context.Message.OrderItems.ForEach(async item =>
            {
                stockResult.Add(await _context.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            });

            if (stockResult.All(x => x.Equals(true)))
            {
                foreach (var item in context.Message.OrderItems)
                {

                    var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                    if (stock != null)
                    {
                        stock.Count -= item.Count;
                    }

                    await _context.SaveChangesAsync();
                }



                _logger.LogInformation($"Stock was reserved for CorrelationId:{context.Message.CorrelationId}");
                StockReservedEvent stockReservedEvent = new StockReservedEvent(context.Message.CorrelationId)
                {
                   
                    OrderItems = context.Message.OrderItems
               };

               
                await _publishEndpoint.Publish(stockReservedEvent);


            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent(context.Message.CorrelationId)
                {
                    
                    Reason = "Not Enough Stock"
                });
                _logger.LogInformation($"Not enough stock for CorrelationId:{context.Message.CorrelationId}");

            }
        }
    }
}

