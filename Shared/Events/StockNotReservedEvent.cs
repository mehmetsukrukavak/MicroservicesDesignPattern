using System;
using Shared.Interfaces;

namespace Shared
{
    public class StockNotReservedEvent : IStockNotReservedEvent
    {

        public StockNotReservedEvent(Guid correltionId)
        {
            CorrelationId = correltionId;
        }

        public Guid CorrelationId { get; }
        public string Reason { get; set; }
    }
}

