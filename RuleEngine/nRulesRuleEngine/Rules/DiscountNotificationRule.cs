using NRules.Fluent.Dsl;
using nRulesRuleEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nRulesRuleEngine.Rules
{
	public class DiscountNotificationRule : Rule
	{
		public override void Define()
		{
			Customer customer = null;

			When()
				.Match<Customer>(() => customer)
				.Exists<Order>(o => o.Customer == customer, o => o.PercentDiscount > 0.0);

			Then()
				.Do(_ => customer.NotifyAboutDiscount());
		}
	}
}
