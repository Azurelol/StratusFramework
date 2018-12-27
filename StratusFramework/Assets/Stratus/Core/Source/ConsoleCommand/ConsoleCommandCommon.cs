using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	public class CommonConsoleCommands : IConsoleCommandHandler
	{
		[ConsoleCommand("quit")]
		public static void Quit()
		{
			Application.Quit();
		}

		[ConsoleCommand("log")]
		public static string Log(string message)
		{
			StratusDebug.Log(message);
			return message;
		}

		[ConsoleCommand("time")]
		public static float time { get { return Time.realtimeSinceStartup; } }
	}

}