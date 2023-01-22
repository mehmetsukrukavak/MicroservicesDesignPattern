using System;
using MassTransit;
using Shared;
using Shared.Interfaces;

namespace Payment.API.Consumers
{
	public class StockReservedRequestPaymentConsumer: IConsumer<IStockReservedRequestPayment>
    {
        private ILogger<StockReservedRequestPaymentConsumer> _logger;
        private readonly IPublishEndpoint _publishEndPoint;

        public StockReservedRequestPaymentConsumer(ILogger<StockReservedRequestPaymentConsumer> logger, IPublishEndpoint publishEndPoint)
        {
            _logger = logger;
            _publishEndPoint = publishEndPoint;
        }

        public async Task Consume(ConsumeContext<IStockReservedRequestPayment> context)
        {
            var balance = 3000m;
            if (balance > context.Message.payment.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.payment.TotalPrice} TL. was withdrawn from Credit Card for UserId : {context.Message.BuyerId}");

                await _publishEndPoint.Publish(new PaymentCompletedEvent(context.Message.CorrelationId));
            }
            else
            {
                _logger.LogInformation($"{context.Message.payment.TotalPrice} TL. wasn't withdrawn from Credit Card for UserId : {context.Message.BuyerId}");

                await _publishEndPoint.Publish(new PaymentFailedEvent(context.Message.CorrelationId) { Reason = "Not enough balance", OrderItems = context.Message.OrderItems });
            }
        }
    }
}

