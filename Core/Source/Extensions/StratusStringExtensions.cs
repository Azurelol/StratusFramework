using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Stratus
{
    public static partial class Extensions
    {
        //--------------------------------------------------------------------/
        // Fields
        //--------------------------------------------------------------------/
        public const char newlineChar = '\n';
        public const char whitespace = ' ';
        public const char underscore = '_';

        //--------------------------------------------------------------------/
        // Methods
        //--------------------------------------------------------------------/
        /// <summary>
        /// Counts the number of lines in this string (by splitting it)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CountLines(this string str)
        {
            return str.Split('\n').Length;
        }

        /// <summary>
        /// Appends a sequence of strings to the end of this string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static string Append(this string str, IEnumerable<string> sequence)
        {
            StringBuilder builder = new StringBuilder(str);
            foreach (string item in sequence)
            {
                builder.Append(item);
            }
            return builder.ToString();
        }

        public static string Append(this string str, params string[] sequence)
        {
            return str.Append((IEnumerable<string>)sequence);
        }

        public static string AppendLine(this string str, string value)
        {
            return str.IsNullOrEmpty() ? value : $"{str}\n{value}";
        }

        public static IEnumerable<string> TrimNullEmpty(this IEnumerable<string> sequence)
        {
            foreach(var item in sequence)
            {
                if (item.IsValid())
                    yield return item;
            }
        }

        public static string[] TrimNullEmpty(this string[] source)
        {
            List<string> sequence = new List<string>();
            foreach (var item in source)
            {
                if (item.IsValid())
                    sequence.Add(item);
            }
            return sequence.ToArray();
        }

        public static IEnumerable<string> TrimNullEmptyOr(this IEnumerable<string> sequence, Predicate<string> predicate)
        {
            foreach (var item in sequence)
            {
                if (item.IsValid() && predicate(item))
                    yield return item;
            }
        }

        public static string[] TrimNullEmptyOr(this string[] source, Predicate<string> predicate)
        {
            List<string> sequence = new List<string>();
            foreach (var item in source)
            {
                if (item.IsValid() && predicate(item))
                    sequence.Add(item);
            }
            return sequence.ToArray();
        }

        /// <summary>
        /// Returns true if the string is null or empty
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Returns true if the string is neither null or empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValid(this string str)
        {
            return !str.IsNullOrEmpty();
        }

        public static string[] Sort(this string[] source)
        {
            string[] destination = new string[source.Length];
            Array.Copy(source, destination, source.Length);
            destination.Sort();
            return destination;
        }

        /// <summary>
        /// Strips all newlines in the string, replacing them with spaces
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceNewLines(this string str, string replacement)
        {
            return str.Replace("\n", replacement);
        }

        /// <summary>
        /// Uppercases the first character of this string
        /// </summary>
        public static string UpperFirst(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        /// <summary>
        /// Concatenates the elements of a specified array or the members of a collection, 
        /// using the specified separator between each element or member.
        /// </summary>
        public static string Join(this IEnumerable<string> str, string separator)
        {
            return string.Join(separator, str);
        }

        /// <summary>
        /// Concatenates the elements of a specified array or the members of a collection, 
        /// using the specified separator between each element or member.
        /// </summary>
        public static string Join(this IEnumerable<string> str, char separator)
        {
            return string.Join(separator.ToString(), str);
        }

        public static string Join(this string str, IEnumerable<string> values)
        {
            return string.Join(str, values);            
        }

        public static string Join(this string str, params string[] values)
        {
            return string.Join(str, values);
        }

        /// <summary>
        /// Concatenates the elements of a specified array or the members of a collection, 
        /// using the newline separator between each element or member.
        /// </summary>
        public static string JoinLines(this IEnumerable<string> str)
        {
            return string.Join("\n", str);
        }

        /// <summary>
        /// Converts a string to camel case. eg: "Hello There" -> "helloThere"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        /// <summary>
        /// Converts a string to title case. eg: "HelloThere" -> "Hello There")
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            StringBuilder builder = new StringBuilder();
            bool previouslyUppercase = false;

            for (int i = 0; i < str.Length; i++)
            {
                char current = str[i];

                if ((current == underscore || current == whitespace)
                    && (i + 1 < str.Length))
                {
                    if (i > 0)
                    {
                        builder.Append(whitespace);
                    }

                    char next = str[i + 1];
                    if (char.IsLower(next))
                    {
                        next = char.ToUpper(next, CultureInfo.InvariantCulture);
                    }
                    builder.Append(next);
                    i++;
                }
                else
                {
                    // Special case for first char
                    if (i == 0)
                    {
                        builder.Append(current.ToUpper());
                        previouslyUppercase = true;
                    }
                    // Upper
                    else if (current.IsUpper())
                    {
                        if (previouslyUppercase)
                        {
                            builder.Append(current.ToLower());
                        }
                        else
                        {
                            builder.Append(whitespace);
                            builder.Append(current);
                            previouslyUppercase = true;
                        }

                    }
                    // Lower
                    else
                    {
                        builder.Append(current);
                        previouslyUppercase = false;
                    }
                }
            }

            return builder.ToString();
            //return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Formats this string, applying rich text formatting to it
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToRichText(this string str, FontStyle style, int size, Color color)
        {
            StringBuilder builder = new StringBuilder();

            switch (style)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Bold:
                    builder.Append("<b>");
                    break;
                case FontStyle.Italic:
                    builder.Append("<i>");
                    break;
                case FontStyle.BoldAndItalic:
                    builder.Append("<b><i>");
                    break;
            }

            builder.Append($"<color=#{color.ToHex()}>");
            builder.Append($"<size={size}>");
            builder.Append(str);
            builder.Append("</size>");
            builder.Append("</color>");

            switch (style)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Bold:
                    builder.Append("</b>");
                    break;
                case FontStyle.Italic:
                    builder.Append("</i>");
                    break;
                case FontStyle.BoldAndItalic:
                    builder.Append("</i></b>");
                    break;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Formats this string, applying rich text formatting to it
        /// </summary>
        public static string ToRichText(this string str, FontStyle style, Color color)
        {
            StringBuilder builder = new StringBuilder();

            switch (style)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Bold:
                    builder.Append("<b>");
                    break;
                case FontStyle.Italic:
                    builder.Append("<i>");
                    break;
                case FontStyle.BoldAndItalic:
                    builder.Append("<b><i>");
                    break;
            }

            // Color
            builder.Append($"<color=#{color.ToHex()}>");
            builder.Append(str);
            builder.Append("</color>");

            switch (style)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Bold:
                    builder.Append("</b>");
                    break;
                case FontStyle.Italic:
                    builder.Append("</i>");
                    break;
                case FontStyle.BoldAndItalic:
                    builder.Append("</i></b>");
                    break;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Formats this string, applying rich text formatting to it
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToRichText(this string str, FontStyle style)
        {
            StringBuilder builder = new StringBuilder();

            switch (style)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Bold:
                    builder.Append("<b>");
                    break;
                case FontStyle.Italic:
                    builder.Append("<i>");
                    break;
                case FontStyle.BoldAndItalic:
                    builder.Append("<b><i>");
                    break;
            }

            // Color
            builder.Append(str);

            switch (style)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Bold:
                    builder.Append("</b>");
                    break;
                case FontStyle.Italic:
                    builder.Append("</i>");
                    break;
                case FontStyle.BoldAndItalic:
                    builder.Append("</i></b>");
                    break;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Formats this string, applying rich text formatting to it
        /// </summary>
        public static string ToRichText(this string str, Color color)
        {
            return str.ToRichText(FontStyle.Normal, color);
        }

        public static bool IsUpper(this char c)
        {
            return char.IsUpper(c);
        }

        public static bool IsLower(this char c)
        {
            return char.IsLower(c);
        }

        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }

        public static char ToLower(this char c)
        {
            return char.ToLower(c);
        }

        /// <summary>
        /// Given a string, if it exceeds the target length,
        /// truncates it, adding ellipsis at its end (3 chars worth)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TruncateWithEllipsis(this string str, int length)
        {
            if (str.Length > length)
            {
                str = $"{str.Substring(0, length-3)}...";
            }
            return str;

        }

    }


}