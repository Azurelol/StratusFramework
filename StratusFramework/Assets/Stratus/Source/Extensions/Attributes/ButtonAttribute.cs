/******************************************************************************/
/*!
@file   ButtonAttribute.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@note:   https://gist.github.com/LotteMakesStuff/dd785ff49b2a5048bb60333a6a125187
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
  public class ButtonAttribute : PropertyAttribute 
  {
    public string ButtonLabel;
    public string MethodName;

    // Set this false to make the button not work whilst in playmode
    public bool isActiveAtRuntime = true;
    // Set this to false to make the button not work when the game isn't running
    public bool isActiveInEditor = true;

    public ButtonAttribute(string buttonLabel, string methodName, int order = 1)
    {
      this.ButtonLabel = buttonLabel;
      this.MethodName = methodName;

      this.order = order; // Defualt the order to 1 so this can draw under headder attributes
    }
  }
}
