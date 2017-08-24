/******************************************************************************/
/*!
@file   MouseDrivenController.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.Events;
using Stratus.Utilities;

namespace Stratus
{
  /// <summary>
  /// Includes components and utilities designed for quick prototypes
  /// </summary>
  namespace Prototyping
  {
    public abstract class MouseDrivenController : MonoBehaviour
    {
      protected abstract void OnLeftMouseButtonDown(RaycastHit hit);
      protected abstract void OnMiddleMouseButtonDown(RaycastHit hit);
      protected abstract void OnRightMouseButtonDown(RaycastHit hit);

      protected abstract void OnUpdate();

      private void FixedUpdate()
      {
        // Check the left, middle and right mouse buttons
        this.CheckMouseButton(0, OnLeftMouseButtonDown);
        this.CheckMouseButton(2, OnMiddleMouseButtonDown);
        this.CheckMouseButton(1, OnRightMouseButtonDown);

        this.OnUpdate();
      }

      /// <summary>
      /// Checks whether the specified mouse button has been hit.
      /// </summary>
      /// <param name="mouseButtonNumber"></param>
      /// <param name="onMouseButtonFunc"></param>
      void CheckMouseButton(int mouseButtonNumber, System.Action<RaycastHit> onMouseButtonFunc)
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

}