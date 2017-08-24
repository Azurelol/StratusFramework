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
  [Singleton("Stratus Overlay", true, true)]
  public partial class Overlay : Singleton<Overlay>
  {
    //------------------------------------------------------------------------/
    // Declarations
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
    /// Displays the current FPS
    /// </summary>
    public static bool showFPS { get; set; } = true;

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The anchored position of the default overlay window
    /// </summary>
    private OverlayWindows Windows = new OverlayWindows();

    /// <summary>
    /// All custom windows written by the user
    /// </summary>
    private Dictionary<string, Window> CustomWindows = new Dictionary<string, Window>();

    /// <summary>
    /// Displays the FPS in the game Window
    /// </summary>
    private FPSCounter fpsCounter = new FPSCounter();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      Reset();
      Scene.onSceneChanged += OnSceneChanged; 
    }

    private void Update()
    {
      if (showFPS) fpsCounter.Update();
    }

    private void OnGUI()
    {
      Draw();      
    }

    //------------------------------------------------------------------------/
    // Methods: Public
    //------------------------------------------------------------------------/
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
      get.Windows.Watch.Add(watcher);      
    }
    /// <summary>
    /// Adds a button to the overlay which invokes a function with no parameters.
    /// </summary>
    /// <param name="description">What description to use for the button</param>
    /// <param name="onButtonDown">The function to be invoked when the button is pressed</param>
    public static void AddButton(string description, Button.Callback onButtonDown)
    {      
      var button = new Button(description, onButtonDown);
      get.Windows.Buttons.Add(button);
    }

    /// <summary>
    /// Adds a button to the overlay which invokes a function with no parameters
    /// </summary>
    /// <param name="onButtonDown"></param>
    public static void AddButton(Button.Callback onButtonDown)
    {
      var button = new Button(onButtonDown.Method.Name, onButtonDown);
      get.Windows.Buttons.Add(button);
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
      get.Windows.Buttons.Add(button);
    }
    

    /// <summary>
    /// Shows the overlay
    /// </summary>
    public static void Show()
    {
      get.enabled = true;
    }    

    /// <summary>
    /// Hides the overlay
    /// </summary>
    public static void Hide()
    {
      get.enabled = false;
    }

    //------------------------------------------------------------------------/
    // Methods: Private
    //------------------------------------------------------------------------/
    /// <summary>
    /// Resets all windows to their defaults
    /// </summary>
    void Reset()
    {
      Windows.Watch = new Window("Watch", new Vector2(0.3f, 0.2f), Color.grey, Anchor.TopRight);
      Windows.Buttons = new Window("Buttons", new Vector2(0.3f, 0.4f), Color.grey, Anchor.BottomRight);
    }

    /// <summary>
    /// When the scene changes, reset all windows!
    /// </summary>
    void OnSceneChanged()
    {      
      Reset();
    }

    /// <summary>
    /// Draws all overlay elements
    /// </summary>
    void Draw()
    {
      // Draw all the innate windows
      Windows.Watch.Draw();
      Windows.Buttons.Draw();     

      // Draw all custom windows
      foreach (var window in CustomWindows)
        window.Value.Draw();

      // Show FPS
      if (showFPS) DisplayFPS();
    }

    void DisplayFPS()
    {
      //GUILayout.BeginArea()
      //GUILayout.Label(new GUIContent(fpsCounter.averageFPSLabel));
    }

  }
}
