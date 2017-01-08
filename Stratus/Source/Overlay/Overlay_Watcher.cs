/******************************************************************************/
/*!
@file   Overlay_Watcher.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus.Utilities;

namespace Stratus
{
  public partial class Overlay
  {
    /// <summary>
    /// Keeps watch over a given variable
    /// </summary>
    public class Watcher : Element
    {
      /// <summary>
      /// A reference to the given variable
      /// </summary>
      private Reflection.VariableReference Variable;

      /// <summary>
      /// The owner of this variable
      /// </summary>
      private MonoBehaviour Behaviour;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="prefix">A given prefix for this variable</param>
      /// <param name="variable">The variable being watched</param>
      public Watcher(Reflection.VariableReference variable, MonoBehaviour behaviour) : base(variable.Name)
      {
        Behaviour = behaviour;
        Variable = variable;
      }

      /// <summary>
      /// Draws the variable to the window
      /// </summary>
      protected override void OnDraw()
      {               
        if (Behaviour != null)
          GUILayout.Label(Behaviour.gameObject.name + "." + Behaviour.GetType().Name + "." + Variable.Name + " = " + Variable.Value);
        else
          GUILayout.Label(Variable.Name + " = " + Variable.Value);
      }
    }
  }
}