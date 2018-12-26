using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public interface IConsoleCommand
	{
		string label { get; set; }
		string description { get; set; }
		string usage { get; set; }
		void Execute(string args);
	}
	
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
	public class ConsoleCommandAttribute : Attribute
	{

	}

	public interface IConsoleCommandHandler
	{

	}		

	public class Boo45 : IConsoleCommandHandler
	{

	}

	public abstract class ConsoleCommand : IConsoleCommand
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public const char delimiter = ' ';
		string IConsoleCommand.label { get; set; }
		string IConsoleCommand.description { get; set; }
		string IConsoleCommand.usage { get; set; }
		public abstract void Execute(string args);

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static Type[] handlerTypes { get; private set; }
		public static IConsoleCommand[] commands { get; private set; }
		public static Dictionary<string, IConsoleCommand> commandsByName { get; private set; }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		static ConsoleCommand()
		{
			RegisterCommands();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static void Submit(string text)
		{
			string[] args = text.Split(delimiter);
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private static void RegisterCommands()
		{
			handlerTypes = Stratus.Utilities.Reflection.GetInterfaces(typeof(IConsoleCommandHandler));
		}

		
	}

}