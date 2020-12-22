using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		public static void SetWidth(this RectTransform rectTransform, float width)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		public static void SetHeight(this RectTransform rectTransform, float height)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
	}

}