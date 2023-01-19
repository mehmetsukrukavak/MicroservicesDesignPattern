using System;
using Shared.Interfaces;

namespace Shared
{
    public class StockReservedEvent : IStockReservedEvent
    {
        public StockReservedEvent(Guid correltionId )
        {
            CorrelationId = correltionId;
        }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
    }
}

