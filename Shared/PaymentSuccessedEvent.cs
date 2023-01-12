using System;
namespace Shared
{
	public class PaymentSuccessedEvent
	{
		public int orderId { get; set; }

		public string BuyerId { get; set; }
	}
}

