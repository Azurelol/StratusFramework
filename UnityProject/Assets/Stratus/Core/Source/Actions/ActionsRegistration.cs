/******************************************************************************/
/*!
@file   ActionsRegistration.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Whenever an Monobehaviour on a GameObject creates an Action,
  /// this component is attached to it at runtime in order to handle book-keeping.
  /// It is only destroyed at the moment the GameObject is being destroyed.
  /// </summary>
  [DisallowMultipleComponent]
  public class ActionsRegistration : MonoBehaviour
  {
    private GameObject owner { get; set; }
    
    void Start()
    {
      // This component is only used for runtime book-keeping
      this.owner = this.gameObject;
      this.hideFlags = HideFlags.HideAndDontSave;
    }
    
    void OnDestroy()
    {
      // When this GameObject dies, it gets deregistered from the actions system.
      if (this.enabled)
        ActionSpace.Unsubscribe(this.owner);
    }

  }

}