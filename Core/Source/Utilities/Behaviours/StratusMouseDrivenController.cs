using UnityEngine;
using UnityEngine.Events;
using Stratus.Utilities;

namespace Stratus
{
	/// <summary>
	/// Includes components and utilities designed for quick prototypes
	/// </summary>
	public abstract class StratusMouseDrivenController : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnLeftMouseButtonDown(RaycastHit hit);
		protected abstract void OnMiddleMouseButtonDown(RaycastHit hit);
		protected abstract void OnRightMouseButtonDown(RaycastHit hit);
		protected abstract void OnUpdate();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Update()
		{
			// Check the left, middle and right mouse buttons
			this.CheckMouseButton(0, OnLeftMouseButtonDown);
			this.CheckMouseButton(2, OnMiddleMouseButtonDown);
			this.CheckMouseButton(1, OnRightMouseButtonDown);
			this.OnUpdate();
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		/// <summary>
		/// Checks whether the specified mouse button has been hit.
		/// </summary>
		/// <param name="mouseButtonNumber"></param>
		/// <param name="onMouseButtonFunc"></param>
		private void CheckMouseButton(int mouseButtonNumber, System.Action<RaycastHit> onMouseButtonFunc)
		{
			if (Input.GetMouseButtonDown(mouseButtonNumber))
			{
				var cast = Camera.main.CastRayFromMouseScreenPosition();
				if (cast.transform)
				{
					onMouseButtonFunc(cast);
				}
			}
		}


	}

}