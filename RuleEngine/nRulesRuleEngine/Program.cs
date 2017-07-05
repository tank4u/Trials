using NRules;
using NRules.Fluent;
using nRulesRuleEngine.Entities;
using nRulesRuleEngine.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nRulesRuleEngine
{
	class Program
	{
		static void Main(string[] args)
		{
			RunNrules();

			Console.Read();
		}

		private static void RunNrules()
		{
			//Load rules
			RuleRepository repository = new RuleRepository();
			repository.Load(x => x.From(typeof(DiscountRule).Assembly));

			//Compile rules
			RuleCompiler compiler = new RuleCompiler();
			var sessionFactory = compiler.Compile(repository.GetRuleSets());

			//Create a working session
			var session = sessionFactory.CreateSession();

			//Load domain model
			var customer = new Customer("John Doe") { IsPreferred = true };
			var order1 = new Order(123456, customer, 2, 25.0);
			var order2 = new Order(123457, customer, 1, 100.0);

			//Insert facts into rules engine's memory
			//session.Insert(customer);
			//session.Insert(order1);
			//session.Insert(order2);

			//Start match/resolve/act cycle
			var rulesExecuted = session.Fire();

			Console.WriteLine("Rules executed: {0}", rulesExecuted.ToString());

			//Insert facts into rules engine's memory
			session.InsertAll(new List<object> { customer, order2, order1 });

			var customer2 = new Customer("Test customer") { IsPreferred = true };
			//var order3 = new Order(1234, customer2, 2, 25.0);
			
			//Insert facts into rules engine's memory
			session.InsertAll(new List<object> { customer2 });

			rulesExecuted = session.Fire();
			Console.WriteLine("Rules executed: {0}", rulesExecuted.ToString());

			
			rulesExecuted = session.Fire();
			Console.WriteLine("Rules executed: {0}", rulesExecuted.ToString());
		}
	}
}
