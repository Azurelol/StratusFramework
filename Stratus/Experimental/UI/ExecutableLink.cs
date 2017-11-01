/******************************************************************************/
/*!
@file   ExecutableLink.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.Events;
using System;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Executes the specified method when opened.
    /// </summary>
    public class ExecutableLink : Link
    {
      public UnityEvent Method = new UnityEvent();

      protected override void OnActivate()
      {
        Method.Invoke();
        this.Deactivate();
      }

      protected override void OnSelect()
      {
      }
      protected override void OnDeselect()
      {
      }

      protected override void OnConfirm()
      {
      }

      protected override void OnCancel()
      {
      }

      protected override void OnNavigate(Navigation dir)
      {
      }

      /// <summary>
      /// Sets the callback for this link, overwriting any previous ones.
      /// </summary>
      /// <param name="call">An UnityAction, lambda expression, etc.</param>
      public void Set(UnityAction call)
      {
        Method.RemoveAllListeners();
        Method.AddListener(call);
      }


    }

  } 
}