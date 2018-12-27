using System;
using NUnit.Framework;
using UnityEngine;

namespace Stratus.Tests
{
	public class ConsoleCommandTests : IConsoleCommandHandler
	{
		private static string lastCommand => ConsoleCommand.lastCommand;

		//------------------------------------------------------------------------/
		// Mocks
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

		[Test]
		public void TestRegistration()
		{
			Debug.Log("Classes that support console commands:");
			Type[] types = ConsoleCommand.handlerTypes;
			Debug.Log(types.TypeNames().JoinLines());

			Debug.Log("Commands:");
			Debug.Log(ConsoleCommand.commands.Names(x => x.ToString()).JoinLines());
		}

		[ConsoleCommand("floatfield")]
		private static float floatField;

		[ConsoleCommand("intProperty")]
		private static int intProperty { get; set; }

		[ConsoleCommand("intProperty2")]
		private static int intProperty2 => 5;

		//------------------------------------------------------------------------/
		// Testing Methods
		//------------------------------------------------------------------------/
		[Test]
		public void TestLog()
		{
			this.AssertCommandResult("log foo", "foo");
		}

		[Test]
		public void TestAdd()
		{
			this.AssertCommandResult("add 2 5", "7");
		}

		[Test]
		public void TestVector()
		{
			this.AssertCommandResult("addvector 3,4,5 1,1,1", new Vector3(4, 5, 6));
		}

		[Test]
		public void TestTime()
		{
			this.AssertCommandResult("time", Time.realtimeSinceStartup, 0.1f);
		}

		private void TestCommand(string text)
		{
			ConsoleCommand.Submit(text);
			Debug.Log(lastCommand);
		}

		private void AssertCommandResult(string text, object expected)
		{
			this.TestCommand(text);
			Assert.AreEqual(ConsoleCommand.latestResult, expected.ToString());
		}

		private void AssertCommandResult(string text, float expected, float delta)
		{
			this.TestCommand(text);
			Assert.AreEqual(float.Parse(ConsoleCommand.latestResult), expected, delta);
		}



	}
}
