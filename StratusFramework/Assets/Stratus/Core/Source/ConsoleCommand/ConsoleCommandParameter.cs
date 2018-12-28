using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace Stratus
{
	public enum ConsoleCommandParameter
	{
		Unsupported,
		Integer,
		Float,
		String,
		Boolean,
		Vector3
	}

	public static class ConsoleCommandParameterExtensions
	{
		public const string booleanTrueAlternative = "on";
		public const string booleanFalseAlternative = "off";
		public const char delimiter = ConsoleCommand.delimiter;

		public static Type ToType(this ConsoleCommandParameter parameter)
		{
			switch (parameter)
			{
				case ConsoleCommandParameter.Integer:
					return typeof(int);
				case ConsoleCommandParameter.Float:
					return typeof(float);
				case ConsoleCommandParameter.String:
					return typeof(string);
				case ConsoleCommandParameter.Boolean:
					return typeof(bool);
				case ConsoleCommandParameter.Vector3:
					return typeof(Vector3);
				default:
					break;
			}
			return null;
		}

		public static bool TryDeduceParameter(Type type, out ConsoleCommandParameter parameter)
		{
			if (type.Equals(typeof(int)))
			{
				parameter = ConsoleCommandParameter.Integer;
				return true;
			}
			else if (type.Equals(typeof(float)))
			{
				parameter = ConsoleCommandParameter.Float;
				return true;
			}
			else if (type.Equals(typeof(string)))
			{
				parameter = ConsoleCommandParameter.String;
				return true;
			}
			else if (type.Equals(typeof(bool)))
			{
				parameter = ConsoleCommandParameter.Boolean;
				return true;
			}
			else if (type.Equals(typeof(Vector3)))
			{
				parameter = ConsoleCommandParameter.Vector3;
				return true;
			}

			parameter = ConsoleCommandParameter.Unsupported;
			return false;
		}

		/// <summary>
		/// Converts a given a string arg to the supported parameter type
		/// </summary>
		/// <param name="arg"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static object Parse(string arg, ConsoleCommandParameter parameter)
		{
			object value = null;
			switch (parameter)
			{
				case ConsoleCommandParameter.Integer:
					value = int.Parse(arg);
					break;
				case ConsoleCommandParameter.Float:
					value = float.Parse(arg);
					break;
				case ConsoleCommandParameter.String:
					value = arg;
					break;
				case ConsoleCommandParameter.Boolean:
					string lowercaseArg = arg.ToLower();
					if (lowercaseArg.Equals(booleanTrueAlternative))
					{
						value = true;
					}
					else if (lowercaseArg.Equals(booleanFalseAlternative))
					{
						value = false;
					}
					else
					{
						value = bool.Parse(arg);
					}
					break;
				case ConsoleCommandParameter.Vector3:
					value = Extensions.ParseVector3(arg);
					break;
			}
			return value;
		}

		public static object[] Parse(IConsoleCommand command, string args)
		{
			return Parse(command, args.Split(ConsoleCommand.delimiter));
		}

		public static object[] Parse(IConsoleCommand command, string[] args)
		{
			if (command.parameters.Length == 0)
			{
				return null;
			}

			int parameterCount = command.parameters.Length;
			if (args.Length < parameterCount)
			{
				throw new ArgumentException("Not enough arguments passed!");
			}
			else if (args.Length > parameterCount && command.parameters.Last() != ConsoleCommandParameter.String)
			{
				throw new ArgumentException("Too many arguments passed!");
			}

			object[] parse = new object[parameterCount];
			for(int i = 0; i < parameterCount; ++i)
			{
				string param = args[i];
				var paramType = command.parameters[i];
				parse[i] = Parse(param, paramType);
			}

			// If the last parameter is a string, add the rest of the args to it
			if (command.parameters.Last() == ConsoleCommandParameter.String 
				&& args.Length != parameterCount)
			{
				int lastIndex = parameterCount - 1;
				parse[lastIndex] = string.Join(delimiter.ToString(), args.Skip(lastIndex));
			}

			return parse;
		}



		public static ConsoleCommandParameter[] DeduceMethodParameters(MethodInfo method)
		{
			List<ConsoleCommandParameter> parameters = new List<ConsoleCommandParameter>();
			foreach(var parameter in method.GetParameters())
			{
				ConsoleCommandParameter consoleParameter; // = ConsoleCommandParameter.Unsupported;
				if (TryDeduceParameter(parameter.ParameterType, out consoleParameter))
				{
					parameters.Add(consoleParameter);
				}
				else
				{
					throw new ArgumentOutOfRangeException($"Unsupported parameter {parameter.ParameterType} in method {method.Name}");
				}
			}
			return parameters.ToArray();
		}

		public static ConsoleCommandParameter[] DeduceParameters(FieldInfo field)
		{
			ConsoleCommandParameter consoleParameter;
			if (TryDeduceParameter(field.FieldType, out consoleParameter))
			{
				return new ConsoleCommandParameter[] { consoleParameter };
			}
			throw new ArgumentOutOfRangeException($"Unsupported parameter type for field {field.FieldType}");
		}

		public static ConsoleCommandParameter[] DeduceParameters(PropertyInfo property)
		{
			ConsoleCommandParameter consoleParameter;
			if (TryDeduceParameter(property.PropertyType, out consoleParameter))
			{
				return new ConsoleCommandParameter[] { consoleParameter };
			}
			throw new ArgumentOutOfRangeException($"Unsupported parameter type for field {property.PropertyType}");
		}

	}

}