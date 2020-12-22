using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stratus
{
	public enum StratusConsoleCommandParameter
	{
		Integer,
		Float,
		String,
		Enum,
		Boolean,
		Vector2,
		Vector3,
		Rect,
		Object
	}

	public class StratusConsoleCommandParameterInformation
	{
		/// <summary>
		/// The qualified type of the parameter
		/// </summary>
		public Type type;
		/// <summary>
		/// The deduced type (whether it is supported)
		/// </summary>
		public StratusConsoleCommandParameter deducedType;

		public StratusConsoleCommandParameterInformation(Type type, StratusConsoleCommandParameter parameter)
		{
			this.type = type;
			this.deducedType = parameter;
		}
	}

	public static class StratusConsoleCommandParameterExtensions
	{
		public const string booleanTrueAlternative = "on";
		public const string booleanFalseAlternative = "off";
		public const char delimiter = StratusConsoleCommand.delimiter;

		public static Type ToType(this StratusConsoleCommandParameter parameter)
		{
			switch (parameter)
			{
				case StratusConsoleCommandParameter.Integer:
					return typeof(int);
				case StratusConsoleCommandParameter.Float:
					return typeof(float);
				case StratusConsoleCommandParameter.String:
					return typeof(string);
				case StratusConsoleCommandParameter.Boolean:
					return typeof(bool);
				case StratusConsoleCommandParameter.Enum:
					return typeof(Enum);
				case StratusConsoleCommandParameter.Vector2:
					return typeof(Vector2);
				case StratusConsoleCommandParameter.Vector3:
					return typeof(Vector3);
				case StratusConsoleCommandParameter.Rect:
					return typeof(Rect);
				case StratusConsoleCommandParameter.Object:
					return typeof(object);
			}
			return null;
		}

		public static bool TryDeduceParameter(Type type, out StratusConsoleCommandParameter parameter)
		{
			if (type.Equals(typeof(int)))
			{
				parameter = StratusConsoleCommandParameter.Integer;
				return true;
			}
			else if (type.Equals(typeof(float)))
			{
				parameter = StratusConsoleCommandParameter.Float;
				return true;
			}
			else if (type.Equals(typeof(string)))
			{
				parameter = StratusConsoleCommandParameter.String;
				return true;
			}
			else if (type.Equals(typeof(bool)))
			{
				parameter = StratusConsoleCommandParameter.Boolean;
				return true;
			}
			else if (type.IsEnum)
			{
				parameter = StratusConsoleCommandParameter.Enum;
				return true;
			}
			else if (type.Equals(typeof(Vector2)))
			{
				parameter = StratusConsoleCommandParameter.Vector2;
				return true;
			}
			else if (type.Equals(typeof(Vector3)))
			{
				parameter = StratusConsoleCommandParameter.Vector3;
				return true;
			}
			else if (type.Equals(typeof(Rect)))
			{
				parameter = StratusConsoleCommandParameter.Rect;
				return true;
			}

			parameter = StratusConsoleCommandParameter.Object;
			return true;
		}

		/// <summary>
		/// Converts a given a string arg to the supported parameter type
		/// </summary>
		/// <param name="arg"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static object Parse(string arg, StratusConsoleCommandParameterInformation info)
		{
			object value = null;
			switch (info.deducedType)
			{
				case StratusConsoleCommandParameter.Integer:
					value = int.Parse(arg);
					break;
				case StratusConsoleCommandParameter.Float:
					value = float.Parse(arg);
					break;
				case StratusConsoleCommandParameter.String:
					value = arg;
					break;
				case StratusConsoleCommandParameter.Boolean:
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
				case StratusConsoleCommandParameter.Enum:
					value = Enum.Parse(info.type, arg);
					break;
				case StratusConsoleCommandParameter.Vector2:
					value = Extensions.ParseVector2(arg);
					break;
				case StratusConsoleCommandParameter.Vector3:
					value = Extensions.ParseVector3(arg);
					break;
				case StratusConsoleCommandParameter.Rect:
					value = Extensions.ParseRect(arg);
					break;
				case StratusConsoleCommandParameter.Object:
					throw new Exception("Submitting parameters for object types is not supported!");
			}
			return value;
		}

		public static object[] Parse(IStratusConsoleCommand command, string args)
		{
			return Parse(command, args.Split(StratusConsoleCommand.delimiter));
		}

		public static object[] Parse(IStratusConsoleCommand command, string[] args)
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
			else if (args.Length > parameterCount && command.parameters.Last().deducedType != StratusConsoleCommandParameter.String)
			{
				throw new ArgumentException("Too many arguments passed!");
			}

			object[] parse = new object[parameterCount];
			for (int i = 0; i < parameterCount; ++i)
			{
				string param = args[i];
				parse[i] = Parse(param, command.parameters[i]);
			}

			// If the last parameter is a string, add the rest of the args to it
			if (command.parameters.Last().deducedType == StratusConsoleCommandParameter.String
				&& args.Length != parameterCount)
			{
				int lastIndex = parameterCount - 1;
				parse[lastIndex] = string.Join(delimiter.ToString(), args.Skip(lastIndex));
			}

			return parse;
		}

		public static StratusConsoleCommandParameterInformation[] DeduceMethodParameters(MethodInfo method)
		{
			List<StratusConsoleCommandParameterInformation> parameters = new List<StratusConsoleCommandParameterInformation>();
			foreach (ParameterInfo parameter in method.GetParameters())
			{
				// = ConsoleCommandParameter.Unsupported;
				if (TryDeduceParameter(parameter.ParameterType, out StratusConsoleCommandParameter deducedType))
				{
					parameters.Add(new StratusConsoleCommandParameterInformation(parameter.ParameterType, deducedType));
				}
				else
				{
					throw new ArgumentOutOfRangeException($"Unsupported parameter {parameter.ParameterType} in method {method.Name}");
				}
			}
			return parameters.ToArray();
		}

		public static StratusConsoleCommandParameterInformation[] DeduceParameters(FieldInfo field)
		{
			if (TryDeduceParameter(field.FieldType, out StratusConsoleCommandParameter consoleParameter))
			{
				return new StratusConsoleCommandParameterInformation[] { new StratusConsoleCommandParameterInformation(field.FieldType, consoleParameter) };
			}
			throw new ArgumentOutOfRangeException($"Unsupported parameter type for field {field.FieldType}");
		}

		public static StratusConsoleCommandParameterInformation[] DeduceParameters(PropertyInfo property)
		{
			if (TryDeduceParameter(property.PropertyType, out StratusConsoleCommandParameter consoleParameter))
			{
				return new StratusConsoleCommandParameterInformation[] { new StratusConsoleCommandParameterInformation(property.PropertyType, consoleParameter) };
			}
			throw new ArgumentOutOfRangeException($"Unsupported parameter type for field {property.PropertyType}");
		}

	}

}