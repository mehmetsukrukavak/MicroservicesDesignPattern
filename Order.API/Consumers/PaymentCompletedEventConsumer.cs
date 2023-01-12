using System;
using MassTransit;
using Order.API.Models;
using Shared;

namespace Order.API.Consumers
{
	public class PaymentCompletedEventConsumer:IConsumer<PaymentCompletedEvent>
	{
        private readonly AppDbContext _context;
        private ILogger<PaymentCompletedEventConsumer> _logger;

        public PaymentCompletedEventConsumer(AppDbContext context, ILogger<PaymentCompletedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.OrderStatus = OrderStatus.Complete;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order (Id={context.Message.OrderId}) status changed : {order.OrderStatus}");
            }
            else
            {
                _logger.LogError($"Order (Id={context.Message.OrderId}) not found");
            }
            
        }
    }
}

