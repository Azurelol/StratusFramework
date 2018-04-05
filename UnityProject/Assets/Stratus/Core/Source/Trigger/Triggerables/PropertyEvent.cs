using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using Stratus.Types;

namespace Stratus
{
  /// <summary>
  /// Provides the ability to provide changes to a specified MonoBehaviour's properties at runtime
  /// </summary>
  public class PropertyEvent : Triggerable
  {
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/    
    public MemberSetterField[] setters;
    
    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      foreach (var property in setters)
        property.Set(this);
    }    

  }
}