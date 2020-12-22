using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// A generic display for multitons
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class MultitonSceneViewDisplay<T> : LayoutSceneViewDisplay
		where T : StratusMultitonBehaviour<T>
	{
		protected virtual bool showInPlayMode { get; } = true;
		protected override bool isValid => showInPlayMode && StratusMultitonBehaviour<T>.hasInstances;
		protected abstract void OnInitializeMultitonState();

		protected override void OnInitializeState()
		{
			OnInitializeMultitonState();
		}

	}
}
