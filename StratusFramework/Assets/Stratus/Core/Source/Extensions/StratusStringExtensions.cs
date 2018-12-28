using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		public const char newlineChar = '\n';

		/// <summary>
		/// Counts the number of lines in this string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static int CountLines(this string str)
		{
			return str.Split('\n').Length;
		}

		/// <summary>
		/// Strips all newlines in the string, replacing them with spaces
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TrimNewlines(this string str)
		{
			return str.Replace('\n', ' ');
		}

		/// <summary>
		/// Converts a string from CamelCase to a human readable format. 
		/// Inserts spaces between upper and lower case letters. 
		/// Also strips the leading "_" character, if it exists.
		/// </summary>
		/// <param name="str"></param>
		/// <returns>A human readable string.</returns>
		public static string FromCamelCase(this string str)
		{
			return Regex.Replace(str, "(\\B[A-Z0-9])", " $1");
		}

		/// <summary>
		/// Converts a string to title case. eg: "HelloThere" -> "Hello There")
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string str)
		{
			var builder = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				var current = str[i];
				if (current == '_' && i + 1 < str.Length)
				{
					var next = str[i + 1];
					if (char.IsLower(next))
					{
						next = char.ToUpper(next, CultureInfo.InvariantCulture);
					}

					builder.Append(next);
					i++;
				}
				else
				{
					builder.Append(current);
				}
			}

			return builder.ToString();
			//return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
		}

		public static string Join(this string[] str, string separator)
		{
			return string.Join(separator, str);
		}

		public static string JoinLines(this string[] str)
		{
			return string.Join("\n", str);
		}

		public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

		/// <summary>
		/// Returns true if the string is neither null or empty
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsValid(this string str) => !str.IsNullOrEmpty();

		/// <summary>
		/// Formats this string, applying rich text formatting to it
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string Style(this string str, Color color, TextStyle style)
		{
			StringBuilder builder = new StringBuilder();

			// Italic
			if ((style & TextStyle.Italic) == TextStyle.Italic)
			{
				builder.Append("<i>");
			}

			// Bold
			if ((style & TextStyle.Bold) == TextStyle.Bold)
			{
				builder.Append("<b>");
			}

			// Color
			builder.Append("<color=#" + color.ToHex() + ">");
			builder.Append(str);
			builder.Append("</color>");

			// Bold
			if ((style & TextStyle.Bold) == TextStyle.Bold)
			{
				builder.Append("</b>");
			}

			// Italic
			if ((style & TextStyle.Italic) == TextStyle.Italic)
			{
				builder.Append("</i>");
			}

			return builder.ToString();
		}

		

	}


}