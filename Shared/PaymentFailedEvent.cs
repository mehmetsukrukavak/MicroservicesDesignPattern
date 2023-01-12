using System;
namespace Shared
{
	public class PaymentFailedEvent
	{
        public int orderId { get; set; }

        public string BuyerId { get; set; }

        public string Message { get; set; }
    }
}

