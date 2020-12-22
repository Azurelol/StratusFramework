using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using Stratus.Types;

namespace Stratus.Gameplay
{
  /// <summary>
  /// Provides the ability to provide changes to a specified MonoBehaviour's properties at runtime
  /// </summary>
  public class StratusPropertyEvent : StratusTriggerable
  {
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/    
    public StratusMemberSetterField[] setters;
    
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