/******************************************************************************/
/*!
@file   StringExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Text;
using System;
using System.Text.RegularExpressions;

namespace Stratus
{
  public static class StringExtensions
  {
    /// <summary>
    /// Counts the number of lines in this string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int Lines(this string str)
    {
      return str.Split('\n').Length;
    }

    /// <summary>
    /// Strips all newlines in the string, replacing them with spaces
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string StripNewlines(this string str)
    {
      return str.Replace('\n', ' ');
    }

    /// <summary>
    /// Formats this string, applying rich text formatting to it
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Style(this string str, Color color, TextStyle style)
    {
      var builder = new StringBuilder();

      // Italic
      if ((style & TextStyle.Italic) == TextStyle.Italic)
        builder.Append("<i>");

      // Bold
      if ((style & TextStyle.Bold) == TextStyle.Bold)
        builder.Append("<b>");

      // Color
      builder.Append("<color=#" + color.ToHex() + ">");
      builder.Append(str);
      builder.Append("</color>");

      // Bold
      if ((style & TextStyle.Bold) == TextStyle.Bold)
        builder.Append("</b>");

      // Italic
      if ((style & TextStyle.Italic) == TextStyle.Italic)
        builder.Append("</i>");

      return builder.ToString();
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
  }


}