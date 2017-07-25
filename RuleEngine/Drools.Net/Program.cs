using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using org.drools.dotnet.compiler;
using org.drools.dotnet.rule;
using org.drools.dotnet;


namespace Drools.Net
{
	class Program
	{
		static WorkingMemory workingMemory;
		static org.drools.FactHandle _currentCust;

		static void Main(string[] args)
		{
			PackageBuilder builder = new PackageBuilder();
			//use Assembly.GetExecutingAssembly().GetManifestResourceNames() to get full name of Drools rule file
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Drools.Net.Rules.SimpleRules.drl");
			builder.AddPackageFromDrl("Drools.Net.Rules.SimpleRules.drl", stream);
			Package pkg = builder.GetPackage();
			RuleBase ruleBase = RuleBaseFactory.NewRuleBase();
			ruleBase.AddPackage(pkg);

			CustomerRecord workingObj = new CustomerRecord()
			{
				Value = 51
			};
			TestRules(ruleBase, workingObj);

			//Old customer
			workingObj = new CustomerRecord()
			{
				Value = 10,
				DaysSinceLastOrder = 100,
				Status = "Active"
			};
			TestRules(ruleBase, workingObj);

			//Welcome BACK Discount
			workingObj = new CustomerRecord()
			{
				Value = 70,
				Status = "Re-Active"
			};
			TestRules(ruleBase, workingObj);

			Console.ReadLine();
		}

		static void modify(CustomerRecord customer)
		{
			Console.WriteLine("Modify fired for : {0}", customer.Value);
		}

		static void TestRules(RuleBase ruleBase, CustomerRecord workingObj)
		{
			if (workingMemory == null)
			{
				workingMemory = ruleBase.NewWorkingMemory();
			}			
			
			
			if (_currentCust == null)
			{
				_currentCust = workingMemory.assertObject(workingObj);
			}
			else
			{
				workingMemory.modifyObject(_currentCust, workingObj);
			}
			workingMemory.fireAllRules();
		}

		
	}
}
