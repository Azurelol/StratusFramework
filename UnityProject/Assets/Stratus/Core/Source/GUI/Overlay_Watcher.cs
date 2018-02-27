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
  public partial class StratusGUI
  {
    protected override bool isPersistent { get { return true; } }

    /// <summary>
    /// Keeps watch over a given variable
    /// </summary>
    public class Watcher : Element
    {
      /// <summary>
      /// A reference to the given variable
      /// </summary>
      private MemberReference Variable;

      /// <summary>
      /// The owner of this variable
      /// </summary>
      private MonoBehaviour Behaviour;

      /// <summary>
      /// A description of the variable
      /// </summary>
      private string Description;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="prefix">A given prefix for this variable</param>
      /// <param name="variable">The variable being watched</param>
      public Watcher(MemberReference variable, string description, MonoBehaviour behaviour) : base(variable.name)
      {
        Behaviour = behaviour;
        Variable = variable;

        // Whether to use the default variable name or a custom description
        if (description != null)
          Description = description;
        else
          Description = variable.name;
      }

      /// <summary>
      /// Draws the variable to the window
      /// </summary>
      protected override void OnDraw()
      {
        //GUILayout.BeginVertical();
        //GUILayout.Label(Variable.Name);
        //GUILayout.EndVertical();
        //
        //GUILayout.BeginVertical();
        //GUILayout.Label(Variable.Value);        
        //GUILayout.EndVertical();
        if (Behaviour != null)
          GUILayout.Label(Behaviour.gameObject.name + "." + Behaviour.GetType().Name + "." + Description + ":  " + Variable.value.ToString());
        else
          GUILayout.Label(Description + ": " + Variable.value.ToString());

      }
    }

    /// <summary>
    /// Window for watching variables
    /// </summary>
    public class WatcherWindow : Window
    {
      public WatcherWindow(string name, Vector2 relativeDimensions, Color color, Anchor position = Anchor.TopLeft) 
        : base(name, relativeDimensions, color, position)        
      {

      }

      protected override void DrawElements()
      {
        // For every watcher, draw 3 
        // Draw 3 columns
        //DrawColumn("Name")
      }

      void DrawColumn(string title, string content)
      {
        GUILayout.BeginVertical();
        GUILayout.Label(title);
        GUILayout.EndVertical();
      }

    }


  }
}