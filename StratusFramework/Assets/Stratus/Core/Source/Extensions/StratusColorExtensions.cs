using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns a copy of the color with a modified alpha
		/// </summary>
		/// <param name="color">The color whose alpha to change</param>
		/// <param name="alpha">The alpha value to set</param>
		public static Color ToAlpha(this Color color, float alpha)
		{
			color = new Color(color.r, color.g, color.b, alpha);
			return color;
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


	}

}