using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
    public interface IStratusLogger
    {
	}

    public static class IStratusLoggerExtensions
    {
		/// <summary>
		/// Prints the given message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void Log(this IStratusLogger logger, object value) => StratusDebug.Log(value, logger, 2);

		/// <summary>
		/// Prints the given warning message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void LogWarning(this IStratusLogger logger, object value) => StratusDebug.LogWarning(value, logger, 2);

		/// <summary>
		/// Prints the given error message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void LogError(this IStratusLogger logger, object value) => StratusDebug.LogError(value, logger, 2);
	}

}