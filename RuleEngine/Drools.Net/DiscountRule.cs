using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.drools.dotnet.compiler;
using org.drools.dotnet.rule;
using org.drools.dotnet;

namespace Drools.Net
{
	public class DiscountRule
	{
		public static void Print(string message)
		{
			Console.WriteLine("From Rule: {0}", message);
		}
	}
}
