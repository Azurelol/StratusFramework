using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus
{
  /// <summary>
  /// Useful effects for an Unity Canvas components
  /// </summary>
  public class CanvasEffectEvent : Triggerable
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public enum Type
    {
      [Tooltip("Fadess the alpha associated with the graphic")]
      Alpha
    }
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Tooltip("The canvas which will be affected")]
    public Canvas canvas;
    [Tooltip("What type of effect to apply")]
    public Type type = Type.Alpha;

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      
    }

    protected override void OnTrigger()
    {
      
    }

    protected override void OnReset()
    {
      
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
  }

}