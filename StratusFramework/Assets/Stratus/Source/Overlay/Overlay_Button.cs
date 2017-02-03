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

namespace Stratus
{
  public partial class Overlay
  {
    /// <summary>
    /// A button with a provided callback function
    /// </summary>
    public class Button : Element
    {
      /// <summary>
      /// The function to call for this button
      /// </summary>
      public Callback Callback;
      public Button(string name, Callback callback) : base(name)
      {
        Callback = callback;
      }

      protected override void OnDraw()
      {
        if (GUILayout.Button(Name, GUI.skin.button))
          Callback();
      }

    }
  }
}