/******************************************************************************/
/*!
@file   CancelLink.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus 
{
  namespace UI
  {
    /// <summary>
    /// Sends a cancel command when confirmed on.
    /// </summary>
    public class CancelLink : Link
    {
      protected override void OnActivate()
      {
        Trace.Script("CANCEL!");
      }

      protected override void OnSelect()
      {
      }
      protected override void OnDeselect()
      {
      }

      protected override void OnConfirm()
      {
        Trace.Script("CANCEL!");
        //this.Deactivate();
      }

      protected override void OnCancel()
      {
      }

      protected override void OnNavigate(Navigation dir)
      {
      }
    }
  }
}
