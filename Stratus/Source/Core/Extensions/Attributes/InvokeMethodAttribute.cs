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
  /// <summary>
  /// A button that invokes a given method belonging to a MonoBehaviour instance of a specified class
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
  public sealed class InvokeMethodButtonAttribute : PropertyAttribute 
  {
    /// <summary>
    /// The type of the class this button belongs to
    /// </summary>
    public Type type { get; private set; }
    /// <summary>
    /// The name of the method this button will invoke
    /// </summary>
    public string methodName { get; private set; }
    /// <summary>
    /// The name to be displayed in the inspector
    /// </summary>
    public string label { get; set; }
    /// <summary>
    /// Set this false to make the button not work whilst in playmode
    /// </summary>
    public bool isActiveAtRuntime { get; set; } = true;
    /// <summary>
    /// Set this to false to make the button not work when the game isn't running 
    /// </summary>
    public bool isActiveInEditor { get; set; } = true;

    /// <param name="type">The class that the method belongs to</param>
    /// <param name="methodName">The name of the void method with no arguments to invoke</param>
    /// <param name="order"></param>
    public InvokeMethodButtonAttribute(Type type, string methodName, int order = 1)
    {
      if (type.BaseType != typeof(MonoBehaviour))
        throw new Exception("The provided type (" + type.Name + ") is not a MonoBehaviour!");      

      this.type = type;      
      this.methodName = methodName;
      this.label = this.methodName;

      // Default the order to 1 so this can draw under header attributes
      this.order = order; 
    }

    public string missingMethodMessage => "The provided method '" + methodName + "' could not be found in the type '" + type.Name + "'";
    


  }
}
