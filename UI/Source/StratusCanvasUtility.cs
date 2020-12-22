using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Stratus.UI
{
	public static class StratusCanvasUtility
	{
		public static EventSystem eventSystem => EventSystem.current;

		public static void SetExplicitNavigation<T>(IList<T> selectables,
			Func<T, Selectable> selectableFunction,
			StratusOrientation orientation)
		{
			for (int i = 0; i < selectables.Count; i++)
			{
				Selectable selectable = selectableFunction(selectables[i]);


				Navigation navigation = new Navigation();

				navigation.mode = Navigation.Mode.Explicit;
				if (selectables.ContainsIndex(i - 1))
				{
					switch (orientation)
					{
						case StratusOrientation.Horizontal:
							navigation.selectOnLeft = selectableFunction(selectables[i - 1]);
							break;
						case StratusOrientation.Vertical:
							navigation.selectOnUp = selectableFunction(selectables[i - 1]);
							break;
						default:
							break;
					}
				}
				if (selectables.ContainsIndex(i + 1))
				{
					switch (orientation)
					{
						case StratusOrientation.Horizontal:
							navigation.selectOnRight = selectableFunction(selectables[i + 1]);
							break;
						case StratusOrientation.Vertical:
							navigation.selectOnDown = selectableFunction(selectables[i + 1]);
							break;
						default:
							break;
					}

				}

				selectable.navigation = navigation;
			}
		}
	}

}