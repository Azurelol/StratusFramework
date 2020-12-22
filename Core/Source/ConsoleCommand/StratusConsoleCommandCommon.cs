using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	public class CommonConsoleCommands : IStratusConsoleCommandProvider
	{
		[StratusConsoleCommand("quit")]
		public static void Quit()
		{
			Application.Quit();
		}

		[StratusConsoleCommand("log")]
		public static string Log(string message)
		{
			return message;
		}

		[StratusConsoleCommand("time")]
		public static float time { get { return Time.realtimeSinceStartup; } }
	}

}