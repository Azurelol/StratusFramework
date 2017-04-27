/******************************************************************************/
/*!
@file   StructExtensions.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus.Utilities;

namespace Stratus
{
  public static class StructExtensions
  {
    /// <summary>
    /// Sets the alpha on the color instantly.
    /// </summary>
    /// <param name="color">The color whose alpha to change</param>
    /// <param name="alpha">The alpha value to set</param>
    public static void SetAlpha(this Color color, float alpha)
    {
      color = new Color(color.r, color.g, color.b, alpha);
    }

    /// <summary>
    /// Checks the specified value is within the range of this vector
    /// </summary>
    /// <param name="range">A vector containing a min-max range.</param>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is within the range, false otherwise</returns>
    public static bool Contains(this Vector2 range, float value)
    {
      if (value > range.x && value < range.y)
        return true;

      return false;
    }

    /// <summary>
    /// Gets the average between the values of the vector.
    /// </summary>
    /// <param name="range">The vector containing two values.</param>
    /// <returns></returns>
    public static float Average(this Vector2 range)
    {
      return ((range.x + range.y) / 2);
    }

    /// <summary>
    /// Retrieves the hex value of the given color
    /// </summary>
    /// <param name="color">The color from which we want to know the hex value</param>
    /// <returns></returns>
    public static string ToHex(this Color color)
    {
      var hex = ColorUtility.ToHtmlStringRGBA(color);
      
      return hex;
      //Trace.Script("hex value = " + hex);
      //string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
      //return hex;
    }

    /// <summary>
    /// Calculates a random position around the specified position
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="minDist"></param>
    /// <param name="maxDist"></param>
    /// <param name="keepVertical"></param>
    /// <returns></returns>
    public static Vector3 RandomPosition(this Vector3 vec, float minDist, float maxDist, bool keepVertical = true)
    {
      var randomPos = vec;
      // Calculate a random radius from the given range
      float radius = Random.Range(minDist, maxDist);
      // Randomly change the x and z values of the position
      randomPos.x += Random.Range(-radius, radius);
      randomPos.z += Random.Range(-radius, radius);
      if (!keepVertical)
        randomPos.y += Random.Range(-radius, radius);

      return randomPos;
    }

    /// <summary>
    /// Sets this color based on the given hex value
    /// </summary>
    /// <param name="color">The color to be set</param>
    /// <param name="hex">A hex value</param>
    public static void Set(this Color color, string hex)
    {
      color = Graphical.HexToColor(hex);
      //// In case the string is formated 0xFFFFFF
      //hex.Replace("0x", "");
      //// In case the string is formated #FFFFFF
      //hex.Replace("#", "");
      //// Assume fully visible unless specified in hex
      //color.r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
      //color.g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
      //color.b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
      //// Only use alpha if string has enough characters
      //if (hex.Length == 8)
      //  color.a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
      //else
      //  color.a = 255;
    }

  }


}