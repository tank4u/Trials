using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;

namespace RulesWF
{
	public static class RulesMediator
	{
		static Dictionary<string, RuleSet> ruleCache;
		static RulesMediator()
		{
			ruleCache = new Dictionary<string, RuleSet>();
		}

		public static void RunRules<T>(T target, string rulesName)
		{
			//RuleSet rules = GetRules(rulesName);
			////validate rules
			//RuleValidation validation = new RuleValidation(typeof(T), null);
			//rules.Validate(validation);

			//if (validation.Errors.Any(e => !e.IsWarning))
			//{
			//	Console.WriteLine("Errors: {0}", validation.Errors.FirstOrDefault());
			//	return;
			//}

			////warnings are errors
			//RuleEngine engine = new RuleEngine(rules, validation);			
			//engine.Execute(target);

			//OR

			RuleSet rules = GetRules(rulesName);
			RuleEngine engine = new RuleEngine(rules, typeof(T));
			engine.Execute(target);
		}

		public static RuleSet GetRules(string ruleSetName)
		{
			if (ruleCache.ContainsKey(ruleSetName))
				return ruleCache[ruleSetName];
			else
			{
				RuleSet rules = GetRuleSet(ruleSetName);
				ruleCache[ruleSetName] = rules;
				return rules;
			}
		}

		static RuleSet GetRuleSet(string ruleFilePath)
		{
			// De-serialize from a .rules file.
			using (XmlTextReader rulesReader = new XmlTextReader(ruleFilePath))
			{
				WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
				var ruleSet = (RuleSet)serializer.Deserialize(rulesReader);
				rulesReader.Close();

				return ruleSet;
			}
		}
	}
}
