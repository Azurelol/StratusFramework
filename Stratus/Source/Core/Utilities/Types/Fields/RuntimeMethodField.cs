using UnityEngine;
using System;

namespace Stratus
{
  [Serializable]
  public class RuntimeMethodField
  {
    ///// <summary>
    ///// The name of the method which this button will invoke
    ///// </summary>
    //public string methodName;

    ///// <summary>
    ///// Set this false to make the button not work whilst in playmode
    ///// </summary>
    //public bool isActiveAtRuntime;
    ///// <summary>
    ///// Set this to false to make the button not work when the game isn't running 
    ///// </summary>
    //public bool isActiveInEditor;
    /// <summary>
    /// The method which this button will invoke
    /// </summary>
    public System.Action[] methods { get; private set; }

    public RuntimeMethodField(System.Action[] methods)
    {
      this.methods = methods;
    }

    public RuntimeMethodField(System.Action method)
    {
      this.methods = new System.Action[] { method };
    }
  } 
}
