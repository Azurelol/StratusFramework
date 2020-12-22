using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Stratus.UI
{
	[RequireComponent(typeof(Canvas))]
	public abstract class StratusCanvasBehaviour : StratusBehaviour
	{
		public Canvas canvas
		{
			get
			{
				if (_canvas == null)
				{
					_canvas = GetComponent<Canvas>();
				}
				return _canvas;
			}
		}
		private Canvas _canvas;
	}

}