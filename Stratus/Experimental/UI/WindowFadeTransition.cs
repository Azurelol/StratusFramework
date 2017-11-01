/******************************************************************************/
/*!
@file   WindowFadeTransition.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using System;
using UnityEngine.UI;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// 
    /// </summary>
    public class WindowFadeTransition : WindowTransition
    {
      /// <summary>
      /// Called upon when the window has initialized.
      /// </summary>
      protected override void OnWindowInitialize()
      {
        //Trace.Script("Fading window instantly!");
        // Fade out the window instantly
        Fade(0.0f, 0.0f);
      }

      /// <summary>
      /// Called upon when the window is about to open.
      /// </summary>
      protected override void OnWindowOpen()
      {
        // Fade in
        Fade(1.0f, this.Duration);
        Trace.Script("Fading in!");
      }

      /// <summary>
      /// Called upon when the window is about to close.
      /// </summary>
      protected override void OnWindowClose()
      {
        // Fade out
        Fade(0.0f, this.Duration);
        Trace.Script("Fading out!");
      }

      /// <summary>
      /// Tweens the alpha of this window and all its children.
      /// </summary>
      /// <param name="alpha"></param>
      /// <param name="duration"></param>
      void Fade(float alpha, float duration)
      {
        foreach (var graphical in GetComponentsInChildren<Graphic>())
        {
          graphical.CrossFadeAlpha(alpha, duration, true);
        }
      }


    }

  } 
}