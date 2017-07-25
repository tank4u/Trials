using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drools.Net
{
	public class CustomerRecord
	{

		public String Id { get; set; }
		public String Status { get; set; }
		public int Value { get; set; }
		public int DaysSinceLastOrder { get; set; }
		public int TotalOrders { get; set; }
		public CustomerRecord()
		{

		}
		public void Print(string message)
		{
			Console.WriteLine("Customer: {0}", message);
		}

	}
}
