using System;
using Microsoft.EntityFrameworkCore;

namespace Order.API.Models
{
	[Owned]
	public class Address
	{
		public string Line { get; set; }
		public string Provience { get; set; }
		public string District { get; set; }
	}
}

