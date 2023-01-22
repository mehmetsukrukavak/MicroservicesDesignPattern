using System;
using Shared.Interfaces;

namespace Shared.Events
{
    public class StockReservedRequestPayment : IStockReservedRequestPayment
    {
        public StockReservedRequestPayment(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public PaymentMessage payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; set; }
        public string BuyerId { get; set; }
    }
}

