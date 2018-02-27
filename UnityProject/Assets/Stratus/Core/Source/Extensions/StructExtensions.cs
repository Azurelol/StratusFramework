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
    /// Returns a copy of the color with a modified alpha
    /// </summary>
    /// <param name="color">The color whose alpha to change</param>
    /// <param name="alpha">The alpha value to set</param>
    public static Color SetAlpha(this Color color, float alpha)
    {
      color = new Color(color.r, color.g, color.b, alpha);
      return color;
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
    }

    /// <summary>
    /// Recomputes this color with a modified saturation value (multiplying the original by a normalized value between 0-1)
    /// </summary>
    /// <param name="rgbColor"></param>
    /// <param name="saturationRatio"></param>
    /// <returns></returns>
    public static Color Saturate(this Color rgbColor, float saturationRatio)
    {
      float h, s, v;
      Color.RGBToHSV(rgbColor, out h, out s, out v);
      return Color.HSVToRGB(h, s * saturationRatio, v);
    }


    /// <summary>
    /// Calculates a random Vector3 starting from this one
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="minDist"></param>
    /// <param name="maxDist"></param>
    /// <param name="keepVertical"></param>
    /// <returns></returns>
    public static Vector3 CalculateRandomPosition(this Vector3 vec, float minDist, float maxDist, bool keepVertical = true)
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
    /// Given a target and a specified distance, calculates the position
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="dist"></param>
    /// <returns></returns>
    public static Vector3 CalculatePositionAtDistanceFromTarget(this Vector3 vec, Vector3 target, float dist)
    {
      Vector3 dir = target - vec;
      return target + (dir * dist);
    }
    


    public enum Vector3Component { x, y, z }

    /// <summary>
    /// Strips one of the components from the vector
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    public static Vector3 StripComponent(this Vector3 vec, Vector3Component component)
    {
      switch (component)
      {
        case Vector3Component.x:
          return new Vector3(0f, vec.y, vec.z);
        case Vector3Component.y:
          return new Vector3(vec.x, 0f, vec.z);
        case Vector3Component.z:
          return new Vector3(vec.x, vec.y, 0f);
      }

      throw new System.Exception("Missing component");
    }

    /// <summary>
    /// Returns a linearly interpolated value from a (other) to itself (b) at a given t (0-1)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float LerpFrom(this float b, float a, float t)
    {
      return (1 - t) * a + t * b;
    }

    /// <summary>
    /// Returns a linearly interpolated value from a (itself) to the target (b) at a given t (0-1)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float LerpTo(this float a, float b, float t)
    {
      return (1 - t) * a + t * b;
    }

  }


}