/******************************************************************************/
/*!
@file   StringExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

public static class StringExtensions
{  
  public static int CountLines(this string str)
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
}

