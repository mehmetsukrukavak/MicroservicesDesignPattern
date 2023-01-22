using System;
using MassTransit;

namespace Shared.Interfaces
{
	public interface IStockReservedRequestPayment:CorrelatedBy<Guid>
	{
        public PaymentMessage payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

		public string BuyerId { get; set; }
	}
}

