using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// Tags the given static method, field or property as being a static command,
	/// registering it to be used. Note that it must be used within a class that implements the
	/// IConsoleCommandHandler interface
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class StratusConsoleCommandAttribute : Attribute, IStratusConsoleCommand
	{
		public string name { get; set; }
		public string description { get; set; }
		public string usage { get; set; }
		public StratusConsoleCommandParameterInformation[] parameters { get; set; }

		public StratusConsoleCommandAttribute()
		{
		}

		public StratusConsoleCommandAttribute(string name)
		{
			this.name = name;
		}

		public StratusConsoleCommandAttribute(string name, string description) : this(name)
		{
			this.description = description;
		}

		public StratusConsoleCommandAttribute(string name, string description, string usage) : this(name, description)
		{
			this.usage = usage;
		}

		public override string ToString()
		{
			return $"{name} ({parameters.ToStringArray().Join(" ")})";
		}
	}

	/// <summary>
	/// Registers this class as being one that supports console command methods/fields/properties. 	
	/// </summary>
	public interface IStratusConsoleCommandProvider
	{
	}

}