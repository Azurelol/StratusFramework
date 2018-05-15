using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// Field that allows custom methods to be set for an inspector window and run
  /// </summary>
  [Serializable]
  public class RuntimeMethodField
  {
    /// <summary>
    /// The method which this button will invoke
    /// </summary>
    public System.Action[] methods { get; private set; }

    public RuntimeMethodField(params System.Action[] methods)
    {
      this.methods = methods;
    }

    public RuntimeMethodField(System.Action method)
    {
      this.methods = new System.Action[] { method };
    }
    
  } 
}
