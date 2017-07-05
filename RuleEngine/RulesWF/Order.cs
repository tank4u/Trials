using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesWF
{
	public class Order
	{
		public string CustomerName { get; set; }
		public int Quantity { get; set; }
		public int Discount { get; set; }
		public bool DiscountGiven { get; set; }

		public Order(int quantity, string customName = "Customer")
		{
			this.Quantity = quantity;
			this.CustomerName = customName;	
		}
	}

	public class Patient
	{
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string ZipCode { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public bool Passed { get; set; }

		public Patient(string name = "Test Patient", int height = 0, int weight = 0)
		{
			Name = name;
			Height = height;
			Weight = weight;
		}
	}
}
