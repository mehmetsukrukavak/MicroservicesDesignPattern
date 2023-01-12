using System;
using MassTransit;
using Shared;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndPoint;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndPoint)
        {
            _logger = logger;
            _publishEndPoint = publishEndPoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 3000m;
            if (balance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL. was withdrawn from Credit Card for UserId : {context.Message.BuyerId}");

                await _publishEndPoint.Publish(new PaymentCompletedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId });
            }
            else
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL. wasn't withdrawn from Credit Card for UserId : {context.Message.BuyerId}");

                await _publishEndPoint.Publish(new PaymentFailedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId, Message ="Not enough balance", OrderItems = context.Message.OrderItems });
            }
        }
    }
}

