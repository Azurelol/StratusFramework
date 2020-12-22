#define STRATUS_TRACE
#if STRATUS_TRACE

using System;
using System.Diagnostics;
using System.Text;

using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Provides debugging facilities
	/// </summary>
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
#endif
	public static class StratusDebug
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether logging is currently enabled
		/// </summary>
		public static bool logging = true;
		/// <summary>
		/// Whether time stamps are enabled
		/// </summary>
		public static bool timeStamp = true;
		/// <summary>
		/// If true, will notify subscribes to <see cref="onLog"/> of logging callbacks
		/// </summary>
		public static bool invokeLogEvent = false;
		/// <summary>
		/// The color of the time stamp
		/// </summary>
		public static Color timestampLabelColor => _timestampLabelColor.Value;
		private static readonly Lazy<Color> _timestampLabelColor = new Lazy<Color>(() => isProSkin ? Color.green.ScaleSaturation(0.5f) : Color.green);
		/// <summary>
		/// The color used of the GameObject of the class being called
		/// </summary>
		public static Color gameObjectLabelColor => _gameObjectLabelColor.Value;
		private static readonly Lazy<Color> _gameObjectLabelColor = new Lazy<Color>(() => isProSkin ? StratusGUIStyles.Colors.azureTriad.first.ScaleSaturation(0.5f) : StratusGUIStyles.Colors.azureTriad.first);
		/// <summary>
		/// The color of the class the function being called belongs to
		/// </summary>
		public static Color classLabelColor => _classLabelColor.Value;
		private static readonly Lazy<Color> _classLabelColor = new Lazy<Color>(() => isProSkin ? StratusGUIStyles.Colors.azureTriad.second.ScaleSaturation(0.5f) : StratusGUIStyles.Colors.azureTriad.second);
		/// <summary>
		/// The color of the function being called
		/// </summary>
		public static Color methodLabelColor => _methodLabelColor.Value;
		private static readonly Lazy<Color> _methodLabelColor = new Lazy<Color>(() => isProSkin ? StratusGUIStyles.Colors.azureTriad.third.ScaleSaturation(0.5f) : StratusGUIStyles.Colors.azureTriad.third);

		/// <summary>
		/// Whether this is running as a player executable rather than in the editor
		/// </summary>
		public static bool isPlayer => !Application.isEditor;
		/// <summary>
		/// Whether the user is using the pro skin (dark thene)
		/// </summary>
		public static bool isProSkin
		{
			get
			{
#if UNITY_EDITOR
				return UnityEditor.EditorGUIUtility.isProSkin;
#else
				return false;
#endif
			}
		}
		/// <summary>
		/// Whether to log the timestamp
		/// </summary>
		public static bool logTimestamp => Application.isPlaying ? timeStamp : false;
		/// <summary>
		/// The singular string builder used for logging
		/// </summary>
		private static StringBuilder builder = new StringBuilder();

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// If event handling is enabled, notifies subscribers whenever logging is done through this class
		/// </summary>
		public static event Action<string, LogType> onLog;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Logs the given message of the given type to the console
		/// </summary>
		public static void Log(object message, LogType logType, object source = null, int frame = 1)
		{
			if (!logging || isPlayer)
			{
				return;
			}

			StackTrace stackTrace = new StackTrace();
			builder.Clear();

			// If timestamp is enabled, prefix the current time
			if (logTimestamp)
			{
				builder.Append(ComposeTimestamp());
			}
			// If there's a provided component reference, get the name of its owner gameobject
			if (source != null)
			{
				builder.Append(ComposeSourcePrefix(source));
			}
			// Get the name of the method and class that called this
			builder.Append(ComposeLogPrefix(stackTrace, frame));
			// Append the message
			builder.Append(message);

			// Now output it to the aporopiate Unity log function
			string output = builder.ToString();
			if (invokeLogEvent)
			{
				onLog?.Invoke(output, logType);
			}
			switch (logType)
			{
				case LogType.Log:
					UnityEngine.Debug.Log(output, source as UnityEngine.Object);
					break;
				case LogType.Error:
					UnityEngine.Debug.LogError(output, source as UnityEngine.Object);
					break;
				case LogType.Assert:
					UnityEngine.Debug.LogAssertion(output, source as UnityEngine.Object);
					break;
				case LogType.Warning:
					UnityEngine.Debug.LogWarning(output, source as UnityEngine.Object);
					break;
				case LogType.Exception:
					throw new NotSupportedException($"Logging exceptions is not supported by this function");
			}
		}

		/// <summary>
		/// Prints the given message to the console, prefixing it with the name of the method
		/// and the class name. (And optionally, its owner GameObject)
		/// </summary>
		/// <param name="message">The message object.</param>
		/// <param name="component">The component which invoked this method. </param>
		/// <param name="color">The color of the message</param>
		public static void Log(object message, object source = null, int frame = 1)
		{
			Log(message, LogType.Log, source, frame + 1);
		}

		/// <summary>
		/// If log is true, prints the given message to the console, prefixing it with the name of the method
		/// and the class name. (And optionally, its owner GameObject)
		/// </summary>
		/// <param name="log"></param>
		/// <param name="message"></param>
		/// <param name="source"></param>
		/// <param name="frame"></param>
		public static void LogIf(bool log, object message, object source = null, int frame = 1)
		{
			if (log)
			{
				Log(message, source, frame + 1);
			}
		}

		/// <summary>
		/// Prints the given message, prefixing it with the name of the method
		/// and the class name. (And optionally, its owner GameObject)
		/// </summary>
		/// <param name="message">The message object.</param>
		/// <param name="component">The component which invoked this method. </param>
		/// <param name="color">The color of the message</param>
		public static void LogWarning(object message, object source = null, int frame = 1)
		{
			Log(message, LogType.Warning, source, frame + 1);
		}

		/// <summary>
		/// Prints the given message, prefixing it with the name of the method
		/// and the class name. This is printed as an error followed by an exception.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="component">The component which invoked the method.</param>
		/// <param name="throwException">Whether to throw an exception.</param>
		public static void LogError(object message, object source = null, int frame = 1)
		{
			Log(message, LogType.Error, source, frame + 1);
		}

		public static void LogErrorBreak(object message, object source = null, int frame = 1)
		{
			LogError(message, source, frame + 1);
			UnityEngine.Debug.Break();
		}

		/// <summary>
		/// Composes the class.method prefix used by the various logging functions
		/// </summary>
		/// <param name="stackTrace"></param>
		/// <param name="frame"></param>
		/// <returns></returns>
		private static string ComposeLogPrefix(StackTrace stackTrace, int frame)
		{
			string methodName = stackTrace.GetFrame(frame).GetMethod().Name;
			string className = stackTrace.GetFrame(frame).GetMethod().ReflectedType.Name;
			string classStr = Format(className, classLabelColor, FontStyle.Bold);
			string methodStr = Format(methodName, methodLabelColor, FontStyle.Bold);
			// Make the prefix
			string prefix = $"{classStr}.{methodStr}: ";
			return prefix;
		}

		/// <summary>
		/// If the source object isn't null, composes a formatted string
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static string ComposeSourcePrefix(object source)
		{
			return $" {Format(source.ToString(), gameObjectLabelColor, FontStyle.BoldAndItalic)} ";
		}

		/// <summary>
		/// Composes a string of text with the timestamp used by logging functions
		/// </summary>
		/// <returns></returns>
		private static string ComposeTimestamp()
		{
			return Format("[" + Math.Round(Time.unscaledTime, 2) + "] ", timestampLabelColor);
		}

		/// <summary>
		/// Displays a modal dialog in Editor
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="ok"></param>
		/// <param name="onOk"></param>
		public static void Dialog(string title, string message, string ok = "OK", System.Action onOk = null)
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorUtility.DisplayDialog(title, message, ok))
			{
				onOk?.Invoke();
			}
#endif
		}

		/// <summary>
		/// Displays a modal dialog in Editor
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="ok"></param>
		/// <param name="cancel"></param>
		/// <param name="onOk"></param>
		public static void Dialog(string title, string message, string ok, string cancel, System.Action onOk = null)
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel))
			{
				onOk?.Invoke();
			}
#endif
		}

		/// <summary>
		/// Formats a string, applying stylying and coloring to it
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="color"></param>
		/// <param name="italic"></param>
		/// <returns></returns>
		private static string Format(object obj, Color color, FontStyle style = FontStyle.Normal)
		{
			if (obj == null)
			{

			}
			return obj.ToString().ToRichText(style, color);
		}
	}
}
#endif

// References:
// http://stackoverflow.com/questions/171970/how-can-i-find-the-method-that-called-the-current-method  
// http://web.archive.org/web/20130124234247/http://abdullin.com/journal/2008/12/13/how-to-find-out-variable-or-parameter-name-in-c.html