/******************************************************************************/
/*!
@file   Overlay.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus.Utilities;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// A programmatic overlay for debugging use. You can use the preset window
  /// for quick prototyping, or make your own windows.
  /// </summary>
  public partial class Overlay : StratusSingleton<Overlay>
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/

    public class OverlayWindows
    {
      public Window Watch;
      public Window Buttons; 
      public Console Console;
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether the overlay is enabled or not
    /// </summary>    
    public bool Enabled = true;
    /// <summary>
    /// The anchored position of the default overlay window
    /// </summary>
    public OverlayWindows Windows = new OverlayWindows();
    /// <summary>
    /// The name of this object
    /// </summary>
    protected override string Name { get { return "Stratus Overlay"; } }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// All custom windows written by the user
    /// </summary>
    private Dictionary<string, Window> CustomWindows = new Dictionary<string, Window>();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      Reset();
      Scene.OnSceneChanged += OnSceneChanged; 
    }

    private void OnGUI()
    {
      if (!Enabled)
        return;

      Draw();      
    }
    
    /// <summary>
    /// Resets all windows to their defaults
    /// </summary>
    void Reset()
    { 
      Windows.Watch = new Window("Watch", new Vector2(0.3f, 0.2f), Color.grey, Anchor.TopRight);      
      Windows.Buttons = new Window("Buttons", new Vector2(0.3f, 0.4f), Color.grey, Anchor.BottomRight);
      //Windows.Console = new Console("Console", new Vector2(0.2f, 0.2f), Color.grey, Anchor.TopRight);
    }

    /// <summary>
    /// When the scene changes, reset all windows!
    /// </summary>
    void OnSceneChanged()
    {
      //Trace.Script("Scene changed!");
      Reset();
    }    

    //------------------------------------------------------------------------/
    // GUI
    //------------------------------------------------------------------------/
    void Draw()
    {
      // Draw all the innate windows
      Windows.Watch.Draw();
      Windows.Buttons.Draw();
      //Windows.Console.Draw();

      // Draw all custom windows
      foreach (var window in CustomWindows)
        window.Value.Draw();
    }


    //------------------------------------------------------------------------/
    // User
    //------------------------------------------------------------------------/
    //public Window MakeWindow(string name, Vector2 dimensions, Color color, Anchor position)
    //{
    //  var window = new Window(name, dimensions, color, position);
    //  CustomWindows.Add(name, window);
    //  return window;
    //}

    /// <summary>
    /// Keeps watch of a given property/field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="varExpr">An expression of a given variable of the form: '(()=> foo')</param>
    /// <param name="behaviour">The owner object of this variable</param>
    /// <example>Overlay.Watch(()=> foo, this);</example>
    public static void Watch<T>(Expression<Func<T>> varExpr, string description = null, MonoBehaviour behaviour = null)
    {
      var variableRef = Reflection.GetReference(varExpr);
      var watcher = new Watcher(variableRef, description, behaviour);
      Instance.Windows.Watch.Add(watcher);      
    }

    ///// <summary>
    ///// Prints a message to the overlay console
    ///// </summary>
    ///// <param name="message"></param>
    //public static void Log(object message)
    //{
    //  Instance.Windows.Console.Log(message);
    //}

    /// <summary>
    /// Adds a button to the overlay which invokes a function with no parameters.
    /// </summary>
    /// <param name="description">What description to use for the button</param>
    /// <param name="onButtonDown">The function to be invoked when the button is pressed</param>
    public static void AddButton(string description, Button.Callback onButtonDown)
    {
      var button = new Button(description, onButtonDown);
      Instance.Windows.Buttons.Add(button);
    }

    /// <summary>
    /// Adds a button to the overlay which invokes a function with any parameters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="description">What description to use for the button</param>
    /// <param name="onButtonDown">The function to be invoked when the button is pressed, using a lambda expresion to pass it: '()=>Foo(7)</param>
    public static void AddButton<T>(string description, Button<T>.Callback onButtonDown)
    {
      var button = new Button<T>(description, onButtonDown);
      Instance.Windows.Buttons.Add(button);
    }

    /// <summary>
    /// Shows the overlay
    /// </summary>
    public static void Show()
    {
      Instance.Enabled = true;
    }    

    /// <summary>
    /// Hides the overlay
    /// </summary>
    public static void Hide()
    {
      Instance.Enabled = false;
    }

  }
}
