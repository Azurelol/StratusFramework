/******************************************************************************/
/*!
@file   Overlay_Button.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  public partial class Overlay
  {
    /// <summary>
    /// A button with a provided callback function
    /// </summary>
    public class Button : Element
    {
      public delegate void Callback();
      /// <summary>
      /// The function to call for this button
      /// </summary>
      public Callback OnButtonDown;
      public Button(string name, Callback onButtonDown) : base(name)
      {
        OnButtonDown = onButtonDown;
      }

      protected override void OnDraw()
      {
        if (GUILayout.Button(name, GUI.skin.button))
          OnButtonDown.DynamicInvoke();
          //Callback();
      }

    }

    public class Button<T> : Element
    {
      public delegate void Callback(T arg);
      Callback OnButtonDown;

      public Button(string name, Callback onButtonDown) : base(name)
      {
        this.OnButtonDown = onButtonDown;
      }

      protected override void OnDraw()
      {
        if (GUILayout.Button(name, GUI.skin.button))
          OnButtonDown.DynamicInvoke();
      }
    }


  }
}