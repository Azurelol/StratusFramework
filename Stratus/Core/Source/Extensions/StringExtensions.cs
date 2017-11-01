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
  /// <summary>
  /// Clears the string
  /// </summary>
  /// <param name="str"></param>
  public static void Clear(this string str)
  {
    str = "";
  }

  public static int CountLines(this string str)
  {
    return str.Split('\n').Length;
  }
}

