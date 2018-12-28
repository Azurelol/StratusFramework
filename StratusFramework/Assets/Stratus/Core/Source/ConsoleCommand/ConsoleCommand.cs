using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stratus
{
	public interface IConsoleCommand
	{
		string name { get; set; }
		string description { get; set; }
		string usage { get; set; }
		ConsoleCommandParameter[] parameters { get; set; }
	}

	public abstract class ConsoleCommand : IConsoleCommand
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public const char delimiter = ' ';
		private static readonly BindingFlags flags =
			BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		private static List<string> _commandHistory = new List<string>();
		private static List<string> _commandResults = new List<string>();
		private static Dictionary<string, Action<string>> commandActions;

		//------------------------------------------------------------------------/
		// Properties: Interface
		//------------------------------------------------------------------------/
		string IConsoleCommand.name { get; set; }
		string IConsoleCommand.description { get; set; }
		string IConsoleCommand.usage { get; set; }
		ConsoleCommandParameter[] IConsoleCommand.parameters { get; set; }

		//------------------------------------------------------------------------/
		// Properties: Static
		//------------------------------------------------------------------------/
		public static Type[] handlerTypes { get; private set; }
		public static Dictionary<string, Type> handlerTypesByName { get; private set; }
		public static IConsoleCommand[] commands { get; private set; }
		public static Dictionary<string, IConsoleCommand> commandsByName { get; private set; }
		public static string[] commandLabels { get; private set; }
		public static string[] variableNames { get; private set; }
		public static string[] commandHistory { get; private set; }
		public static string lastCommand => commandHistory.LastOrDefault();
		public static string latestResult => _commandResults.LastOrDefault();

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
		public static bool Submit(string text)
		{
			RecordHistory(text);

			string[] args = text.Split(delimiter);

			if (args.Length < 1)
			{
				return false;
			}
			else if (args.Length == 1)
			{
				string commandName = args.First();
				Action<string> commandAction = commandActions.GetValueOrNull(commandName);
				if (commandAction != null)
				{
					try
					{
						commandAction.Invoke(null);
					}
					catch (Exception e)
					{
						RecordHistory($"Error parsing the command {commandsByName[commandName]} ({e.Message})");
					}
					return true;
				}
			}
			else
			{
				string commandName = args.First();
				for (int i = 1; i < args.Length; ++i)
				{
					Action<string> commandAction = commandActions.GetValueOrNull(commandName);
					if (commandAction != null)
					{

						try
						{
							string arg = string.Join(delimiter.ToString(), args.Skip(i));
							commandAction(arg);
						}
						catch (Exception e)
						{
							RecordHistory($"Error parsing the command {commandsByName[commandName]} ({e.Message})");
						}
						return true;
					}
					commandName += (delimiter + args[i]);
				}
			}

			RecordHistory($"No command that matches {text} could be found!");
			return false;
		}

		public static void ClearHistory()
		{
			_commandHistory.Clear();
			commandHistory = _commandHistory.ToArray();
		}

		private static void RecordHistory(string text)
		{
			_commandHistory.Add(text);
			commandHistory = _commandHistory.ToArray();
		}

		private static void RecordResult(string text, object result)
		{
			RecordHistory(text);
			_commandResults.Add(result.ToString());
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private static void RegisterCommands()
		{
			commandsByName = new Dictionary<string, IConsoleCommand>();
			commandActions = new Dictionary<string, Action<string>>();

			List<IConsoleCommand> commands = new List<IConsoleCommand>();
			List<string> variableNames = new List<string>();

			handlerTypes = Stratus.Utilities.Reflection.GetInterfaces(typeof(IConsoleCommandHandler));
			handlerTypesByName = new Dictionary<string, Type>();
			handlerTypesByName.AddRange(handlerTypes, x => x.Name);

			foreach (Type handler in handlerTypes)
			{
				// Methods
				foreach (MethodInfo method in handler.GetMethods(flags))
				{
					TryAddCommand(method, (command) =>
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceMethodParameters(method);
						commandActions.Add(command.name, (string args) =>
						{
							object[] parameters = Parse(command, args);
							object returnValue = method.Invoke(null, parameters);
							if (returnValue != null)
							{
								RecordResult($"{command.name}({parameters.Names().Join(",")}) = {returnValue}", $"{returnValue}");
							}
						});
					});
				}

				// Fields
				foreach (FieldInfo field in handler.GetFields(flags))
				{
					TryAddCommand(field, (command) =>
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceParameters(field);
						ConsoleCommandParameter parameter = command.parameters[0];
						commandActions.Add(command.name, (string args) =>
						{
							bool hasValue = args.IsValid();
							if (hasValue)
							{
								field.SetValue(null, Parse(parameter, args));
							}
							else
							{
								object value = field.GetValue(null);
								RecordResult($"{command.name} = {value}", value);
							}
						});

					});
				}

				// Properties
				foreach (PropertyInfo property in handler.GetProperties(flags))
				{
					TryAddCommand(property, (command) =>
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceParameters(property);
						ConsoleCommandParameter parameter = command.parameters[0];

						bool hasSetter = property.GetSetMethod(true) != null;
						if (hasSetter)
						{
							commandActions.Add(command.name, (string args) =>
							{
								bool hasValue = args.IsValid();
								if (hasValue)
								{
									property.SetValue(null, Parse(parameter, args));
								}
								else if (parameter == ConsoleCommandParameter.Boolean)
								{
									bool previousValue = (bool)property.GetValue(null);
									property.SetValue(null, !previousValue);
								}
								else
								{
									object value = property.GetValue(null);
									RecordResult($"{command.name} = {value}", value);
								}
							});
						}
						else
						{
							commandActions.Add(command.name, (args) =>
							{
								bool hasValue = args.IsValid();
								if (hasValue)
								{
									RecordHistory($"{command.name} has no setters!");
								}
								else
								{
									object value = property.GetValue(null);
									RecordResult($"{command.name} = {value}", value);
								}
							});
						}

					});
				}
			}

			IConsoleCommand TryAddCommand(MemberInfo member, Action<IConsoleCommand> onCommandAdded)
			{
				IConsoleCommand command = member.GetAttribute<ConsoleCommandAttribute>();
				if (command != null)
				{
					onCommandAdded(command);
					commandsByName.Add(command.name, command);
					commands.Add(command);
				}
				return command;
			}

			ConsoleCommand.variableNames = variableNames.ToArray();
			ConsoleCommand.commands = commands.ToArray();
		}

		public static object Parse(ConsoleCommandParameter parameter, string arg)
		{
			return ConsoleCommandParameterExtensions.Parse(arg, parameter);
		}

		public static object[] Parse(IConsoleCommand command, string args)
		{
			return ConsoleCommandParameterExtensions.Parse(command, args);
		}

		public static object[] Parse(IConsoleCommand command, string[] args)
		{
			return ConsoleCommandParameterExtensions.Parse(command, args);
		}
	}

}