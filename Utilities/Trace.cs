/******************************************************************************/
/*!
@file   Trace.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Stratus
{
  // Reference:
  // http://stackoverflow.com/questions/171970/how-can-i-find-the-method-that-called-the-current-method
  public class Trace
  {
    //static Color MonoBehaviourColor = Color.o

    /**************************************************************************/
    /*!
    @brief  Returns the name of the method that called this.
    @param  frame How far up the stack to look.
    @return The name of the method.
    */
    /**************************************************************************/
    static public string Function(int frame = 1)
    {
      StackTrace stackTrace = new StackTrace();
      return stackTrace.GetFrame(frame).GetMethod().Name;      
    }
    
    /// <summary>
    /// Prints the given message, prefixing it with the name of the Method
    /// and the class name.
    /// </summary>
    /// <param name="obj">The message object.</param>
    /// <param name="component">The component which invoked this method. </param>
    static public void Script(object obj, MonoBehaviour component = null)
    {
      StackTrace stackTrace = new StackTrace();
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      var timeElapsed = "<color=green>[" + Time.unscaledTime + "]</color> ";
      // If there's a provided component, use it as a prefix in the trace

      var prefix = "<b>" + className + "." + methodName + "</b>: ";
      
      if (component)
      {
        var gameObjName = "<i><color=orange>" + component.gameObject.name + ".</color></i>";
        UnityEngine.Debug.Log(timeElapsed + gameObjName + prefix + obj);
      }
      else 
        UnityEngine.Debug.Log(timeElapsed + prefix + obj);
    }
    
    /// <summary>
    /// Prints the name of the method and class which called this function.
    /// </summary>
    /// <param name="depth">How far up the stack to look. </param>
    /// <param name="component">The component which invoked this method. </param>
    static public void Caller(int depth = 2, MonoBehaviour component = null)
    {
      StackTrace stackTrace = new StackTrace();
      // Request method
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      // Caller method
      var callerMethodName = stackTrace.GetFrame(depth).GetMethod().Name;
      var callerClassName = stackTrace.GetFrame(depth).GetMethod().ReflectedType.Name;

      var prefix = "<b><color=green>" + callerClassName + "." + callerMethodName 
                   + "</color>." + className + "." + methodName + "</b>: ";      

      if (component)
      {
        var gameObjName = "<i><color=orange>" + component.gameObject.name + ".</color></i>";
        UnityEngine.Debug.Log(gameObjName + prefix);
      }
      else
        UnityEngine.Debug.Log(prefix);
    }

    /// <summary>
    /// Prints the given message, prefixing it with the name of the Method
    /// and the class name. This is printed as an error followed by an exception.
    /// </summary>
    /// <param name="obj">The data to log.</param>
    /// <param name="component">The component which invoked the method.</param>
    /// <param name="throwException">Whether to throw an exception.</param>
    public static void Error(object obj, MonoBehaviour component = null, bool throwException = false)
    {
      StackTrace stackTrace = new StackTrace();
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      var timeElapsed = "<color=green>[" + Time.unscaledTime + "]</color> ";
      // If there's a provided component, use it as a prefix in the trace

      var prefix = "<b>" + className + "." + methodName + "</b>: ";

      if (component)
      {
        var gameObjName = "<i><color=orange>" + component.gameObject.name + ".</color></i>";
        UnityEngine.Debug.LogError(timeElapsed + gameObjName + prefix + obj);
      }
      else
        UnityEngine.Debug.LogError(timeElapsed + prefix + obj);

      if (throwException)
        throw new Exception("Operation halted!");
    }

    void Log(object message)
    {

    }

    void Error(object message)
    {

    }
    

    /**************************************************************************/
    /*!
    @brief  Prints the value of the given variable as well its owner,
            all the way up to the GameObject that invoked it.
    @param  obj The message.
    @param  this A reference to the component.
    */
    /**************************************************************************/
    static public void MemberValue<T>(Expression<Func<T>> varExpr)
    {
      StackTrace stackTrace = new StackTrace();
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;      
      var body = ((MemberExpression)varExpr.Body);
      var varName = body.Member.Name;
      var varValue = ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value);
      UnityEngine.Debug.Log(className + "." + methodName + "." + varName + " = '" + varValue + "'");
    }

    /**************************************************************************/
    /*!
    @brief  Prints the value of the given variable as well its owner,
            all the way up to the GameObject that invoked it.
    @param  obj The message.
    @param  this A reference to the component.
    */
    /**************************************************************************/
    static public void Field<T>(Expression<Func<T>> varExpr, MonoBehaviour component)
    {
      StackTrace stackTrace = new StackTrace();
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      var gameObjName = component.gameObject.name;
      var body = ((MemberExpression)varExpr.Body);
      var varName = body.Member.Name;
      var varValue = ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value);
      UnityEngine.Debug.Log(gameObjName + "." + className + "." + methodName + "." 
                            + varName + " = '" + varValue + "'");
    }

    // Reference:
    // http://web.archive.org/web/20130124234247/http://abdullin.com/journal/2008/12/13/how-to-find-out-variable-or-parameter-name-in-c.html


  }

}

