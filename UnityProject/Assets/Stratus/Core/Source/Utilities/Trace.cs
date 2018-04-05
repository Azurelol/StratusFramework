/******************************************************************************/
/*!
@file   Trace.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
//#define STRATUS_TRACE
#if STRATUS_TRACE

using UnityEngine;
using System.Diagnostics;
using System;
using System.Text;

namespace Stratus
{
  [Flags]
  public enum TextStyle
  {
    None = 1,
    Bold = 2,
    Italic = 4
  }

  // References:
  // http://stackoverflow.com/questions/171970/how-can-i-find-the-method-that-called-the-current-method  
  // http://web.archive.org/web/20130124234247/http://abdullin.com/journal/2008/12/13/how-to-find-out-variable-or-parameter-name-in-c.html

  /// <summary>
  /// Provides logging facilities
  /// </summary>
  #if UNITY_EDITOR
  [UnityEditor.InitializeOnLoad]
  #endif
  public static class Trace
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether tracing is currently enabled
    /// </summary>
    public static bool enabled = true;
    /// <summary>
    /// What font to use
    /// </summary>
    public static Font font;
    /// <summary>
    /// Whether time stamps are enabled
    /// </summary>
    public static bool timeStamp = true;
    /// <summary>
    /// The color of the function being called
    /// </summary>
    public static Color methodColor = Color.black;
    /// <summary>
    /// The color of the class the function being called belongs to
    /// </summary>
    public static Color classColor = Color.black;
    /// <summary>
    /// The color used of the GameObject of the class being called
    /// </summary>
    public static Color gameObjectColor = Color.blue;
    /// <summary>
    /// The color of the time stamp
    /// </summary>
    public static Color timeStampColor = Color.green;

    //------------------------------------------------------------------------/
    // Default Start
    //------------------------------------------------------------------------/
    /// <summary>
    /// Resets to the default color and styles
    /// </summary>
    static public void Reset()
    {
      methodColor = Color.black;
      classColor = Color.black;
      gameObjectColor = Color.blue;
      timeStampColor = Color.cyan;
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Returns the name of the method that called this function
    /// </summary>
    /// <param name="frame">How far up the stack to go</param>
    /// <returns></returns>
    static public string Function(int frame = 1)
    {
      StackTrace stackTrace = new StackTrace();
      return stackTrace.GetFrame(frame).GetMethod().Name;      
    }
    
    /// <summary>
    /// Prints the given message, prefixing it with the name of the method
    /// and the class name. (And optionally, its owner GameObject)
    /// </summary>
    /// <param name="message">The message object.</param>
    /// <param name="component">The component which invoked this method. </param>
    /// <param name="color">The color of the message</param>
    static public void Script(object message, Behaviour component = null, Color? color = null)
    {
      if (!enabled)
        return;
      
      var builder = new StringBuilder();
      StackTrace stackTrace = new StackTrace();
      // If timestamp is enabled, prefix the current time
      if (timeStamp)
      {
        var timeElapsed = Format("[" + Math.Round(Time.unscaledTime, 2) + "] ", timeStampColor);
        builder.Append(timeElapsed);
      }
      // If there's a provided component reference, get the name of its owner gameobject
      if (component)
      {
        var gameObjName = Format(component.gameObject.name, gameObjectColor, TextStyle.Bold | TextStyle.Italic);
        builder.Append(" " + gameObjName + " ");
      }
      // Get the name of the method and class that called this
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      var methodStr = Format(methodName, methodColor, TextStyle.Bold);
      var classStr = Format(className, classColor, TextStyle.Bold);
      // Make the prefix
      var prefix = classStr + "." + methodStr + ": ";
      builder.Append(prefix);
      // Append the message
      builder.Append(message);
      // Now print the message
      UnityEngine.Debug.Log(builder.ToString());
    }

    /// <summary>
    /// Prints the given message, prefixing it with the name of the method
    /// and the class name. This is printed as an error followed by an exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="component">The component which invoked the method.</param>
    /// <param name="throwException">Whether to throw an exception.</param>
    public static void Error(object message, Behaviour component = null, bool throwException = false)
    {
      if (!enabled)
        return;

      var builder = new StringBuilder();
      StackTrace stackTrace = new StackTrace();

      // 1. If timestamp is enabled, prefix the current time
      if (timeStamp)
      {
        var timeElapsed = Format("[" + Time.unscaledTime + "]", timeStampColor);
        builder.Append(timeElapsed);
      }

      // 2. If there's a provided component reference, get the name of its owner gameobject
      if (component)
      {
        var gameObjName = Format(component.gameObject.name, gameObjectColor, TextStyle.Italic);
        builder.Append(" " + gameObjName + " ");
      }

      // 3. Get the name of the method and class that called this
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      var prefix = Format(className + "." + methodName + ": ", classColor, TextStyle.Bold);
      builder.Append(prefix + message);

      // 4. Now print the message
      UnityEngine.Debug.LogError(builder.ToString());

      if (throwException)
        throw new Exception();
    }
    
    /// <summary>
    /// Formats a string, applying stylying and coloring to it
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="color"></param>
    /// <param name="italic"></param>
    /// <returns></returns>
    private static string Format(object obj, Color color, TextStyle style = TextStyle.None)
    {
      var builder = new StringBuilder();

      // Italic
      if ((style & TextStyle.Italic) == TextStyle.Italic)
        builder.Append("<i>");

      // Bold
      if ((style & TextStyle.Bold) == TextStyle.Bold)
        builder.Append("<b>");

      // Color
      builder.Append("<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">");
      builder.Append(obj);
      builder.Append("</color>");

      // Bold
      if ((style & TextStyle.Bold) == TextStyle.Bold)
        builder.Append("</b>");

      // Italic
      if ((style & TextStyle.Italic) == TextStyle.Italic)
        builder.Append("</i>");

      return builder.ToString();
    }

    /// <summary>
    /// Displays a modal dialog in Editor
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="ok"></param>
    /// <param name="onOk"></param>
    public static void Dialog(string title, string message, string ok = "OK", System.Action onOk = null)
    {
      #if UNITY_EDITOR
      if (UnityEditor.EditorUtility.DisplayDialog(title, message, ok))
        onOk?.Invoke();
      #endif
    }

    /// <summary>
    /// Displays a modal dialog in Editor
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="ok"></param>
    /// <param name="cancel"></param>
    /// <param name="onOk"></param>
    public static void Dialog(string title, string message, string ok, string cancel, System.Action onOk = null)
    {
      #if UNITY_EDITOR
      if (UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel))
        onOk?.Invoke();
      #endif
    }    

  }
}
#endif