using System;
namespace Shared
{
	public class RabbitMQSettings
	{
		public const string StockReservedEventQueueName = "stock-reserved-queue";
		public const string OrderCreatedEventQueueName = "stock-order-created-queue";
		public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue";
	}
}

