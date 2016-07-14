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

    /**************************************************************************/
    /*!
    @brief  Prints the given message, as is.
    @param  obj The message.
    */
    /**************************************************************************/
    static public void Log(object obj)
    {
      UnityEngine.Debug.Log(obj);      
    }

    /**************************************************************************/
    /*!
    @brief  Prints the given message, prefixing it with the name of the Method
            and the class name.
    @param  obj The message.
    */
    /**************************************************************************/
    static public void Script(object obj, MonoBehaviour component = null)
    {
      StackTrace stackTrace = new StackTrace();
      var methodName = stackTrace.GetFrame(1).GetMethod().Name;
      var className = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
      // If there's a provided component, use it as a prefix in the trace
      if (component)
      {
        var gameObjName = component.gameObject.name;
        UnityEngine.Debug.Log(gameObjName + "." + className + "." + methodName + ": " + obj);
      }
      else 
        UnityEngine.Debug.Log(className + "." + methodName + ": " + obj);
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

