using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Workflow.Activities.Rules;
using System.Workflow.Activities.Rules.Design;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;

namespace RulesWF
{
    class Program
    {
		const string RuleFile = "testRules.rules";
		static string ruleFilePath = Path.Combine(Environment.CurrentDirectory, RuleFile);
		static string patientRuleFilePath = Path.Combine(Environment.CurrentDirectory, "PatientRules.rules");
		static RuleSet ruleSet = null;
		static RuleSet patientRuleSet = null;

		static void Main(string[] args)
		{
			CheckRulesFile(patientRuleFilePath);
			CheckRulesFile(ruleFilePath);

			SetupPatientRules();
			SetupRules();					

			ExecuteRules();

			Console.Read();
		}

		static void ExecuteRules()
		{
			// Execute the rules and print the entity's properties
			Order myOrder = new Order(30, "Customer 1");
			RunRuleOnOrder(myOrder);

			Order secondOrder = new Order(10, "Customer 2");
			RunRuleOnOrder(secondOrder);

			List<Patient> patients = new List<Patient>()
			{
						 new Patient("Patient 1", 10),
						 new Patient("Patient 2", 0, 10),
						 new Patient("Patient 3", 10, 10),
						 new Patient("Patient 4"),
			};

			Parallel.ForEach(patients, p =>
			{
				RunRuleOnPatient(p);
				Console.WriteLine("{0} status: {1} on Thread:{2}", p.Name, p.Passed, Thread.CurrentThread.ManagedThreadId);
			});

			//Patient newPatient = new Patient("Patient 1", 10);
			//RunRuleOnPatient(newPatient);

			//Patient patient2 = new Patient("Patient 2", 10, 10);
			//RunRuleOnPatient(patient2);
		}

		static void RunRuleOnPatient(Patient patient)
		{
			RulesMediator.RunRules<Patient>(patient, patientRuleFilePath);
			//RuleValidation validation = new RuleValidation(typeof(Patient), null);

			//patientRuleSet.Validate(validation);

			//if (validation.Errors.Any())
			//{
			//	Console.WriteLine("Errors: {0}", validation.Errors.FirstOrDefault());
			//	//return;
			//}

			//RuleExecution execution = new RuleExecution(validation, patient);

			//patientRuleSet.Execute(execution);
		}

		static void RunRuleOnOrder(Order myOrder)
		{
			RulesMediator.RunRules<Order>(myOrder, ruleFilePath);
			//RuleValidation validation = new RuleValidation(typeof(Order), null);

			//ruleSet.Validate(validation);

			//if (validation.Errors.Any())
			//{
			//	Console.WriteLine("Errors: {0}", validation.Errors.FirstOrDefault());
			//	return;
			//}

			//RuleExecution execution = new RuleExecution(validation, myOrder);

			//ruleSet.Execute(execution);
		}

		static void CheckRulesFile(string ruleFilePath)
		{
			if (!File.Exists(ruleFilePath))
			{
				File.Create(ruleFilePath);
			}
		}

		static void SetupRules()
		{
			ruleSet = GetRules(ruleFilePath);

			// Create a RuleSet that works with Orders (just another .net Object)
			RuleSetDialog ruleSetDialog = new RuleSetDialog(typeof(Order), null, ruleSet);
			
			// Show the RuleSet Editor
			ruleSetDialog.ShowDialog();
			
			// Get the RuleSet after editing
			ruleSet = ruleSetDialog.RuleSet;

			Console.WriteLine("Rule created :{0}", ruleSet.Rules.LastOrDefault().Condition);

			SaveRules(ruleSet, ruleFilePath);
		}

		static void SetupPatientRules()
		{
			patientRuleSet = GetRules(patientRuleFilePath);

			// Create a RuleSet that works with Orders (just another .net Object)
			RuleSetDialog ruleSetDialog = new RuleSetDialog(typeof(Patient), null, patientRuleSet);

			// Show the RuleSet Editor
			ruleSetDialog.ShowDialog();

			// Get the RuleSet after editing
			patientRuleSet = ruleSetDialog.RuleSet;

			Console.WriteLine("Rule created :{0}", patientRuleSet.Rules.LastOrDefault().Condition);

			SaveRules(patientRuleSet, patientRuleFilePath);
		}

		static void SaveRules(RuleSet ruleSet, string ruleFilePath)
		{
			// Serialize to a .rules file

			WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();

			XmlWriter rulesWriter = XmlWriter.Create(ruleFilePath);

			serializer.Serialize(rulesWriter, ruleSet);

			rulesWriter.Close();
		}

		static RuleSet GetRules(string ruleFilePath)
		{
			// De-serialize from a .rules file.

			XmlTextReader rulesReader = new XmlTextReader(ruleFilePath);

			WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();

			var ruleSet = (RuleSet)serializer.Deserialize(rulesReader);

			rulesReader.Close();

			return ruleSet;

			//return ruleSet;
		}
    }
}
