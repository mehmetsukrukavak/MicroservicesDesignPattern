﻿using System;
namespace Shared.Interfaces
{
	public interface IOrderCreatedRequestEvent
	{
        public int OrderId { get; set; }
        public string BuyerId { get; set; }

        public PaymentMessage Payment { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}


