/******************************************************************************/
/*!
@file   Trace.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Diagnostics;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;

#if UNITY_EDITOR
using UnityEditor; 
#endif

namespace Stratus
{
  // References:
  // http://stackoverflow.com/questions/171970/how-can-i-find-the-method-that-called-the-current-method  
  // http://web.archive.org/web/20130124234247/http://abdullin.com/journal/2008/12/13/how-to-find-out-variable-or-parameter-name-in-c.html

  /// <summary>
  /// Provides logging facilities
  /// </summary>
  #if UNITY_EDITOR
  [InitializeOnLoad]
  #endif
  public static class Trace
  {
    //------------------------------------------------------------------------/
    // Enumeration
    //------------------------------------------------------------------------/
    [Obsolete("Not used due to using the color picker instead")]
    public enum TextColor
    {
      Black,
      White,
      Grey,
      Red,
      Blue,
      Green,
      Yellow,
      Orange
    }
    
    [Flags]
    public enum TextStyle
    {
      None = 1,
      Bold = 2,
      Italic = 4
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether tracing is currently enabled
    /// </summary>
    public static bool Enabled = true;
    /// <summary>
    /// What font to use
    /// </summary>
    public static Font Font;
    /// <summary>
    /// Whether time stamps are enabled
    /// </summary>
    public static bool TimeStamp = true;
    /// <summary>
    /// The color of the function being called
    /// </summary>
    public static Color MethodColor = Color.black;
    /// <summary>
    /// The color of the class the function being called belongs to
    /// </summary>
    public static Color ClassColor = Color.black;
    /// <summary>
    /// The color used of the GameObject of the class being called
    /// </summary>
    public static Color GameObjectColor = Color.blue;
    /// <summary>
    /// The color of the time stamp
    /// </summary>
    public static Color TimeStampColor = Color.cyan;

    //------------------------------------------------------------------------/
    // Default Start
    //------------------------------------------------------------------------/
    static Trace()
    {
      // Set defaults
      //Reset();
    }

    /// <summary>
    /// Resets to the default color and styles
    /// </summary>
    static public void Reset()
    {
      MethodColor = Color.black;
      ClassColor = Color.black;
      GameObjectColor = Color.blue;
      TimeStampColor = Color.cyan;
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
      if (!Enabled)
        return;
      
      var builder = new StringBuilder();
      StackTrace stackTrace = new StackTrace();

      // If timestamp is enabled, prefix the current time
      if (TimeStamp)
      {
        var timeElapsed = Format("[" + Time.unscaledTime + "] ", TimeStampColor);
        builder.Append(timeElapsed);
      }
      // If there's a provided component reference, get the name of its owner gameobject
      if (component)
      {
        var gameObjName = Format(component.gameObject.name, GameObjectColor, TextStyle.Bold | TextStyle.Italic);
        builder.Append(" " + gameObjName + " ");
      }
      // Get the name of the method and class that called this
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      var methodStr = Format(methodName, MethodColor, TextStyle.Bold);
      var classStr = Format(className, ClassColor, TextStyle.Bold);
      // Make the prefix
      var prefix = classStr + "." + methodStr + ": ";
      builder.Append(prefix);
      // Append the message
      builder.Append(message);
      // Now print the message
      UnityEngine.Debug.Log(builder.ToString());
    }
    
    /// <summary>
    /// Prints the name of the method and class which called this function.
    /// </summary>
    /// <param name="depth">How far up the stack to look. </param>
    /// <param name="component">The component which invoked this method. </param>
    //static public void Caller(int depth = 2, MonoBehaviour component = null)
    //{
    //  if (!Enabled)
    //    return;
    //
    //  var builder = new StringBuilder();
    //  StackTrace stackTrace = new StackTrace();
    //  // Request method
    //  var methodName = stackTrace.GetFrame(1).GetMethod().Name;
    //  var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
    //  var methodStr = Format(methodName, MethodColor, TextStyle.Bold);
    //  var classStr = Format(className, ClassColor, TextStyle.Bold);
    //  // Caller method
    //  var callerMethodName = stackTrace.GetFrame(depth).GetMethod().Name;
    //  var callerClassName = stackTrace.GetFrame(depth).GetMethod().ReflectedType.Name;
    //  var callerMethodStr = Format(callerMethodName, MethodColor, TextStyle.Bold);
    //  var callerClassStr = Format(callerClassName, ClassColor, TextStyle.Bold);
    //  // Concatenate the prefix
    //  var prefix = callerClassName + "." + callerMethodName
    //               + " -> " + className + "." + methodName;
    //  builder.Append(prefix);
    //
    //  // If there's a provided component reference, get the name of its owner gameobject
    //  if (component)
    //  {
    //    var gameObjName = Format(component.gameObject.name, GameObjectColor, TextStyle.Italic);
    //    builder.Append(" " + gameObjName + " ");
    //  }
    //
    //  // Now print the message
    //  UnityEngine.Debug.Log(builder.ToString());
    //}

    /// <summary>
    /// Prints the given message, prefixing it with the name of the method
    /// and the class name. This is printed as an error followed by an exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="component">The component which invoked the method.</param>
    /// <param name="throwException">Whether to throw an exception.</param>
    public static void Error(object message, Behaviour component = null, bool throwException = false)
    {
      if (!Enabled)
        return;

      var builder = new StringBuilder();
      StackTrace stackTrace = new StackTrace();

      // 1. If timestamp is enabled, prefix the current time
      if (TimeStamp)
      {
        var timeElapsed = Format("[" + Time.unscaledTime + "]", TimeStampColor);
        builder.Append(timeElapsed);
      }

      // 2. If there's a provided component reference, get the name of its owner gameobject
      if (component)
      {
        var gameObjName = Format(component.gameObject.name, GameObjectColor, TextStyle.Italic);
        builder.Append(" " + gameObjName + " ");
      }

      // 3. Get the name of the method and class that called this
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      var prefix = Format(className + "." + methodName + ": ", ClassColor, TextStyle.Bold);
      builder.Append(prefix + message);

      // 4. Now print the message
      UnityEngine.Debug.LogError(builder.ToString());

      if (throwException)
        throw new Exception();
    }

    /// <summary>
    /// Prints the value of the given variable as well its owner,
    /// all the way up to the GameObject that invoked it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="varExpr"></param>
    //static public void MemberValue<T>(Expression<Func<T>> varExpr)
    //{      
    //  StackTrace stackTrace = new StackTrace();
    //  var methodName = stackTrace.GetFrame(1).GetMethod().Name;
    //  var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
    //  var body = ((MemberExpression)varExpr.Body);
    //  var varName = body.Member.Name;
    //  var varValue = ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value);
    //  UnityEngine.Debug.Log(className + "." + methodName + "." + varName + " = '" + varValue + "'");
    //}

    /// <summary>
    /// Prints the value of the given field as well its owner,
    /// all the way up to the GameObject that invoked it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="varExpr"></param>
    /// <param name="component"></param>
    //static public void Field<T>(Expression<Func<T>> varExpr, MonoBehaviour component)
    //{
    //  StackTrace stackTrace = new StackTrace();
    //  var methodName = stackTrace.GetFrame(1).GetMethod().Name;
    //  var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
    //  var gameObjName = component.gameObject.name;
    //  var body = ((MemberExpression)varExpr.Body);
    //  var varName = body.Member.Name;
    //  var varValue = ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value);
    //  UnityEngine.Debug.Log(gameObjName + "." + className + "." + methodName + "."
    //                        + varName + " = '" + varValue + "'");
    //}

    /// <summary>
    /// Formats a string, applying stylying and coloring to it
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="color"></param>
    /// <param name="italic"></param>
    /// <returns></returns>
    static string Format(object obj, Color color, TextStyle style = TextStyle.None)
    {
      var builder = new StringBuilder();

      // Italic
      if ((style & TextStyle.Italic) == TextStyle.Italic)
        builder.Append("<i>");

      // Bold
      if ((style & TextStyle.Bold) == TextStyle.Bold)
        builder.Append("<b>");

      // Color
      builder.Append("<color=#" + color.ToHex() + ">");
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

    public static void Dialog(string title, string message, string ok = "OK", System.Action onOk = null)
    {
      #if UNITY_EDITOR
      if (EditorUtility.DisplayDialog(title, message, ok))
        onOk?.Invoke();
      #endif
    }

    public static void Dialog(string title, string message, string ok, string cancel, System.Action onOk = null)
    {
      #if UNITY_EDITOR
      if (EditorUtility.DisplayDialog(title, message, ok, cancel))
        onOk?.Invoke();
      #endif
    }

    ///// <summary>
    ///// Formats a string, applying stylying and coloring to it
    ///// </summary>
    ///// <param name="obj"></param>
    ///// <param name="color"></param>
    ///// <param name="italic"></param>
    ///// <returns></returns>
    //static string Format(object obj, TextColor color, TextStyle style = TextStyle.None)
    //{
    //  var builder = new StringBuilder();

    //  // Italic
    //  if ((style & TextStyle.Italic) == TextStyle.Italic)
    //    builder.Append("<i>");

    //  // Bold
    //  if ((style & TextStyle.Bold) == TextStyle.Bold)
    //    builder.Append("<b>");

    //  // Color
    //  builder.Append("<color=");
    //  switch (color)
    //  {
    //    case TextColor.Black:
    //      builder.Append("black>");
    //      break;
    //    case TextColor.White:
    //      builder.Append("white>");
    //      break;
    //    case TextColor.Grey:
    //      builder.Append("grey>");
    //      break;
    //    case TextColor.Red:
    //      builder.Append("red>");
    //      break;
    //    case TextColor.Blue:
    //      builder.Append("blue>");
    //      break;
    //    case TextColor.Green:
    //      builder.Append("green>");
    //      break;
    //    case TextColor.Yellow:
    //      builder.Append("yellow>");
    //      break;
    //    case TextColor.Orange:
    //      builder.Append("orange>");
    //      break;
    //  }
    //  builder.Append(obj);
    //  builder.Append("</color>");

    //  // Italic
    //  if ((style & TextStyle.Italic) == TextStyle.Italic)
    //    builder.Append("</i>");

    //  // Bold
    //  if ((style & TextStyle.Bold) == TextStyle.Bold)
    //    builder.Append("</b>");

    //  return builder.ToString();
    //}

  }
}