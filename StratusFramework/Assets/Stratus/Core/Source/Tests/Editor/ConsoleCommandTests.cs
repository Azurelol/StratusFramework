using System;
using NUnit.Framework;
using UnityEngine;

namespace Stratus.Tests
{
	public class ConsoleCommandTests : IConsoleCommandHandler
	{
		private static string lastCommand => ConsoleCommand.lastCommand;

		//------------------------------------------------------------------------/
		// Registration
		//------------------------------------------------------------------------/
		[Test]
		public void TestRegistration()
		{
			Debug.Log("Classes that support console commands:");
			Type[] types = ConsoleCommand.handlerTypes;
			Debug.Log(types.TypeNames().JoinLines());

			Debug.Log("Commands:");
			Debug.Log(ConsoleCommand.commands.Names(x => x.ToString()).JoinLines());
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[ConsoleCommand("add")]
		public static int Add(int a, int b)
		{
			return (a + b);
		}

		[ConsoleCommand("addvector")]
		public static string AddVector(Vector3 a, Vector3 b)
		{
			return (a + b).ToString();
		}

		[ConsoleCommand("multfloat")]
		public static string MultiplyFloat(float a, float b, float c)
		{
			return (a * b * c).ToString();
		}

		[ConsoleCommand("flipbool")]
		public static bool FlipBoolean(bool value)
		{
			return !value;
		}

		[Test]
		public void TestLogMethod()
		{
			this.AssertCommandResult("log foo", "foo");
		}

		[Test]
		public void TestMethods()
		{
			this.AssertCommandResult("add 2 5", "7");
			float a = 3, b = 5, c = 7;
			this.AssertCommandResult($"multfloat {a} {b} {c}", a * b * c);
			this.AssertCommandResult("addvector 3,4,5 1,1,1", new Vector3(4, 5, 6));
			bool d = false;
			this.AssertCommandResult($"flipbool {d}", !d);
		}

		//------------------------------------------------------------------------/
		// Variables
		//------------------------------------------------------------------------/
		[ConsoleCommand(nameof(floatField))]
		private static float floatField;

		[ConsoleCommand(nameof(intField))]
		private static int intField;

		[ConsoleCommand(nameof(boolField))]
		private static bool boolField;

		[ConsoleCommand(nameof(stringField))]
		private static string stringField;

		[ConsoleCommand(nameof(vector3Field))]
		private static Vector3 vector3Field;

		[ConsoleCommand(nameof(intProperty))]
		private static int intProperty { get; set; }

		[ConsoleCommand(nameof(intGetProperty))]
		private static int intGetProperty => 5;

		[Test]
		public void TestFields()
		{
			this.AssertMemberSet(nameof(floatField), 5f);
			this.AssertMemberSet(nameof(intField), 7);
			this.AssertMemberSet(nameof(boolField), true);
			this.AssertMemberSet(nameof(boolField), true);
			this.AssertMemberSet(nameof(stringField), "Hello there brown cat");
			this.AssertMemberSet(nameof(vector3Field), new Vector3(1f, 5.5f, 8.9f));
		}

		[Test]
		public void TestProperties()
		{
			this.AssertMemberSet(nameof(intProperty), 25);
			this.AssertCommandResult(nameof(intGetProperty), 5);
			this.AssertGetProperty(nameof(intGetProperty), 25);
		}

		[Test]
		public void TestTimeProperty()
		{
			this.AssertCommandResult("time", Time.realtimeSinceStartup, 0.1f);
		}

		//------------------------------------------------------------------------/
		// Test Procedures
		//------------------------------------------------------------------------/
		private void TestCommand(string text)
		{
			ConsoleCommand.Submit(text);
			Debug.Log(lastCommand);
		}

		private void AssertCommandResult(string text, object expected)
		{
			this.TestCommand(text);
			Assert.AreEqual(expected.ToString(), ConsoleCommand.latestResult);
		}

		private void AssertMemberSet(string memberName, object value)
		{
			this.TestCommand($"{memberName} {value}");
			this.TestCommand($"{memberName}");
			Assert.AreEqual(value.ToString(), ConsoleCommand.latestResult);
		}

		private void AssertGetProperty(string memberName, object value)
		{
			this.TestCommand($"{memberName} {value}");
			this.TestCommand($"{memberName}");
			Assert.AreNotEqual(value.ToString(), ConsoleCommand.latestResult);
		}

		private void AssertCommandResult(string text, float expected, float delta)
		{
			this.TestCommand(text);
			Assert.AreEqual(expected, float.Parse(ConsoleCommand.latestResult), delta);
		}



	}
}
