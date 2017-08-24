/******************************************************************************/
/*!
@file   Actions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stratus
{
  /// <summary>
  /// Interface class that the client will be using for constructing action sets
  /// </summary>
  public static class Actions
  {
    public static bool Debugging = false;

    /// <summary>
    /// Cancels all active actions of the ActionsOwner.
    /// </summary>
    /// <param name="owner">A reference to the owner of this action sequence.</param>
    public static void Cancel(ActionsOwner owner)
    {
      owner.Clear();
    }

    /// <summary>
    /// Cancels all active actions in the component.
    /// </summary>
    /// <param name="component"></param>
    public static void Cancel(MonoBehaviour component)
    {
      ActionSpace.Clear(component);
    }

    /// <summary>
    /// Creates an action sequence.
    /// </summary>
    /// <param name="owner">A reference to the owner of this action sequence.</param>
    /// <returns>An ActionSet object, used for Action constructors.</returns>
    public static ActionSet Sequence(ActionsOwner owner)
    {
      // Construct the sequence
      ActionSet sequence = new ActionSequence();
      // Add it to the owner
      owner.Add(sequence);
      // Return it
      return sequence;
    }

    /// <summary>
    /// Creates an action sequence.
    /// </summary>
    /// <param name="owner">The component which is creating the action.</param>
    /// <returns>An ActionSet object, used for Action constructors.</returns>
    public static ActionSet Sequence(MonoBehaviour component)
    {
      // Construct the sequence
      ActionSet sequence = new ActionSequence();
      // Add it to the owner
      component.gameObject.Actions().Add(sequence);
      // Return it
      return sequence;
    }

    /// <summary>
    /// Creates an action group, a set which runs all actions in parallel
    /// </summary>
    /// <param name="owner">A reference to the owner of this action sequence.</param>
    /// <returns></returns>
    public static ActionSet Group(ActionsOwner owner)
    {
      // Construct the sequence
      ActionSet sequence = new ActionGroup();
      // Add it to the owner
      owner.Add(sequence);
      // Return it
      return sequence;
    }

    /// <summary>
    /// Creates an ActionDelay and adds it to the specified set.
    /// </summary>
    /// <param name="set">A reference to the ActionSet that this action belongs to.</param>
    /// <param name="duration"> duration How long should the delay run for.</param>
    public static Action Delay(ActionSet set, float duration)
    {
      Action delay = new ActionDelay(duration);
      set.Add(delay);
      return delay;
    }

    /// <summary>
    /// Adds a function to be invoked as part of the action set, alongside any arguments. 
    /// Optionally, it also can add a delay before the function is invoked.
    /// </summary>
    /// <param name="set">A reference to the action set.</param>
    /// <param name="func">The function to which to call.</param>
    public static Action Call(ActionSet set, ActionCall.Delegate func, float delay = 0.0f)
    {
      Action call = new ActionCall(func);
      // Optionally, add a delay
      if (delay != 0.0f)
        Delay(set, delay);
      set.Add(call);
      return call;
    }
    
    /// <summary>
    /// Adds a trace, adding it to the specified set.
    /// </summary>
    /// <param name="set">A reference to the set.</param>
    /// <param name="message">The message which to print.</param>
    public static Action Trace(ActionSet set, object message)
    {
      Action trace = new ActionTrace(message);
      set.Add(trace);
      return trace;
    }

    /// <summary>
    /// Destroys the specified object after a set amount of time
    /// </summary>
    /// <param name="set"></param>
    /// <param name="obj"></param>
    /// <param name="delay"></param>
    public static void Destroy(ActionSet set, UnityEngine.Object obj, float delay = 0.0f)
    {
      Call(set, ()=>GameObject.Destroy(obj, delay));
    }       

    /// <summary>
    /// Adds a property change to the action set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="set">A reference to the set.</param>
    /// <param name="varExpr">A lambda expression encapsulating a reference to the property which will be modified</param>
    /// <param name="value">The new value for the property</param>
    /// <param name="duration">Over how long should the property be changed</param>
    /// <param name="ease">What interpolation algorithm to use</param>
    public static void Property<T>(ActionSet set, Expression<Func<T>> varExpr, T value, float duration, Ease ease)
    {
      var memberExpr = varExpr.Body as MemberExpression;
      var inst = memberExpr.Expression;
      var variableName = memberExpr.Member.Name;
      var targetObj = Expression.Lambda<Func<object>>(inst).Compile()();

      // Construct an action then branch depending on whether the member to be
      // interpolated is a property or a field
      Action action = null;

      // Property
      var property = targetObj.GetType().GetProperty(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
      if (property != null)
      {
        var propertyType = property.PropertyType;

        if (propertyType == typeof(float))
        {
          action = new ActionPropertyFloat(targetObj, property, Convert.ToSingle(value), duration, ease);
        }
        else if (propertyType == typeof(int))
        {
          action = new ActionPropertyInt(targetObj, property, Convert.ToInt32(value), duration, ease);
        }
        else if (propertyType == typeof(bool))
        {
          action = new ActionPropertyBool(targetObj, property, Convert.ToBoolean(value), duration, ease);
        }
        else if (propertyType == typeof(Vector2))
        {
          action = new ActionPropertyVector2(targetObj, property, (Vector2)Convert.ChangeType(value, typeof(Vector2)), duration, ease);
        }
        else if (propertyType == typeof(Vector3))
        {
          action = new ActionPropertyVector3(targetObj, property, (Vector3)Convert.ChangeType(value, typeof(Vector3)), duration, ease);
        }
        else if (propertyType == typeof(Vector4))
        {
          action = new ActionPropertyVector4(targetObj, property, (Vector4)Convert.ChangeType(value, typeof(Vector4)), duration, ease);
        }
        else if (propertyType == typeof(Color))
        {
          action = new ActionPropertyColor(targetObj, property, (Color)Convert.ChangeType(value, typeof(Color)), duration, ease);
        }
        else if (propertyType == typeof(Quaternion))
        {
          action = new ActionPropertyQuaternion(targetObj, property, (Quaternion)Convert.ChangeType(value, typeof(Quaternion)), duration, ease);
        }
        else
        {
          Stratus.Trace.Script("Couldn't find the property!");
        }
      }      
      // Field
      else
      {
        var field = targetObj.GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        var fieldType = field.FieldType;

        if (fieldType == typeof(float))
        {
          action = new ActionPropertyFloat(targetObj, field, Convert.ToSingle(value), duration, ease);
        }
        else if (fieldType == typeof(int))
        {
          action = new ActionPropertyInt(targetObj, field, Convert.ToInt32(value), duration, ease);
        }
        else if (fieldType == typeof(bool))
        {
          action = new ActionPropertyBool(targetObj, field, Convert.ToBoolean(value), duration, ease);
        }
        else if (fieldType == typeof(Vector2))
        {
          action = new ActionPropertyVector2(targetObj, field, (Vector2)Convert.ChangeType(value, typeof(Vector2)), duration, ease);
        }
        else if (fieldType == typeof(Vector3))
        {
          action = new ActionPropertyVector3(targetObj, field, (Vector3)Convert.ChangeType(value, typeof(Vector3)), duration, ease);
        }
        else if (fieldType == typeof(Vector4))
        {
          action = new ActionPropertyVector4(targetObj, field, (Vector4)Convert.ChangeType(value, typeof(Vector4)), duration, ease);
        }
        else if (fieldType == typeof(Color))
        {
          action = new ActionPropertyColor(targetObj, field, (Color)Convert.ChangeType(value, typeof(Color)), duration, ease);
        }
        else if (fieldType == typeof(Quaternion))
        {
          action = new ActionPropertyQuaternion(targetObj, field, (Quaternion)Convert.ChangeType(value, typeof(Quaternion)), duration, ease);
        }
        else
        {
          Stratus.Trace.Script("Couldn't find the field!");
        }
      }
      // Now add it!
      set.Add(action);
    }

  }

}

