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
using System.Collections;

using System.Reflection;
using System.Linq.Expressions;

namespace Stratus {

  /**************************************************************************/
  /*!
  @class Actions Interface class that the client will be using for constructing 
         and interacting with actions.
  */
  /**************************************************************************/
  public static class Actions {
    public static bool Debugging = false;

    /**************************************************************************/
    /*!
    @brief Creates an action sequence.
    @param owner A reference to the owner of this action sequence.
    @return An ActionGroup object, used for Action constructors.
    */
    /**************************************************************************/
    public static ActionSet Sequence(ActionsOwner owner)
    {
      // Construct the sequence
      ActionSet sequence = new ActionSequence();
      // Add it to the owner
      owner.Add(sequence);
      // Return it
      return sequence;
    }

    public static ActionSet Sequence(MonoBehaviour component)
    {
      // Construct the sequence
      ActionSet sequence = new ActionSequence();
      // Add it to the owner
      component.gameObject.Actions().Add(sequence);
      // Return it
      return sequence;
    }

    /**************************************************************************/
    /*!
    @brief Creates an action group.
    @param owner A reference to the owner of this action sequence.
    @return An ActionGroup object, used for Action constructors.
    */
    /**************************************************************************/
    public static ActionSet Group(ActionsOwner owner)
    {
      // Construct the sequence
      ActionSet sequence = new ActionGroup();
      // Add it to the owner
      owner.Add(sequence);
      // Return it
      return sequence;
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionDelay and adds it to the specified set.
    @param set A reference to the ActionSet that this action belongs to.
    @param duration How long should the delay run for.
    */
    /**************************************************************************/
    public static void Delay(ActionSet set, float duration)
    {
      Action delay = new ActionDelay(duration);
      set.Add(delay);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionCall and adds it to the specified set.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Call(ActionSet set, ActionCall.Delegate func)
    {
      Action call = new ActionCall(func);
      set.Add(call);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionTrace and adds it to the specified set.
    @param set A reference to the set.
    @param message The message which to print.
    */
    /**************************************************************************/
    public static void Trace(ActionSet set, object message)
    {
      Action trace = new ActionTrace(message);
      set.Add(trace);
    }

    // REF WRAPPER VERSION

    /**************************************************************************/
    /*!
    @brief Creates an ActionProperty for a 'float' property.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Property(ActionSet set, Real property, float value, float duration, Ease ease)
    {
      Action actionProperty = new ActionPropertyReal(property, value, duration, ease);
      set.Add(actionProperty);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionProperty for a 'Real2' property.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Property(ActionSet set, Real2 property, Vector2 value, float duration, Ease ease)
    {
      Action actionProperty = new ActionReal2Property(property, value, duration, ease);
      set.Add(actionProperty);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionProperty for a 'Real3' property.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Property(ActionSet set, Real3 property, Vector3 value, float duration, Ease ease)
    {
      Action actionProperty = new ActionReal3Property(property, value, duration, ease);
      set.Add(actionProperty);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionProperty for a 'Real4' property.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Property(ActionSet set, Real4 property, Vector4 value, float duration, Ease ease)
    {
      Action actionProperty = new ActionReal4Property(property, value, duration, ease);
      set.Add(actionProperty);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionProperty for a 'Integer' property.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Property(ActionSet set, Integer property, int value, float duration, Ease ease)
    {
      Action actionProperty = new ActionPropertyInteger(property, value, duration, ease);
      set.Add(actionProperty);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionProperty for a 'Boolean' property.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Property(ActionSet set, Boolean property, bool value, float duration, Ease ease)
    {
      //if (Trace)
      //  Debug.Log("");

      Action actionProperty = new ActionBooleanProperty(property, value, duration, ease);
      set.Add(actionProperty);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionColor.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Color(ActionSet set, Renderer renderer, Color value, float duration, Ease ease)
    {
      Action actionColor = new ActionPropertyColorMaterial(renderer, value, duration, ease);
      set.Add(actionColor);
    }

    public static void Color(ActionSet set, SpriteRenderer renderer, Color value, float duration, Ease ease) 
    {
      Action actionColor = new ActionPropertyColorSprite(renderer, value, duration, ease);
      set.Add(actionColor);
    }

    public static void Color(ActionSet set, MaskableGraphic graphic, Color value, float duration, Ease ease)
    {
      Action actionColor = new ActionPropertyColorMaskableGraphic(graphic, value, duration, ease);
      set.Add(actionColor);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionPropertyVector3.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Translate(ActionSet set, Transform transform, Vector3 value, float duration, Ease ease)
    {
      Action actionColor = new ActionPropertyTransformPosition(transform, value, duration, ease);
      set.Add(actionColor);
    }

    public static void Scale(ActionSet set, Transform transform, Vector3 value, float duration, Ease ease)
    {
      Action actionColor = new ActionPropertyTransformScale(transform, value, duration, ease);
      set.Add(actionColor);
    }

    public static void Rotate(ActionSet set, Transform transform, Vector3 value, float duration, Ease ease)
    {
      Action actionColor = new ActionPropertyTransformRotation(transform, value, duration, ease);
      set.Add(actionColor);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionPropertyDelegate
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    //public static void Property(ActionSet set, ActionPropertyDelegateFloat.SetterFunc setter, float initialValue, float value, float duration, Ease ease)
    //{
    //  Action actionColor = new ActionPropertyDelegateFloat(setter, initialValue, value, duration, ease);
    //  set.Add(actionColor);
    //}

    public static void Property<T>(ActionSet set, Expression<Func<T>> varExpr, T value, float duration, Ease ease)
    {
      var memberExpr = varExpr.Body as MemberExpression;
      var inst = memberExpr.Expression;
      var variableName = memberExpr.Member.Name;      
      var targetObj = Expression.Lambda<Func<object>>(inst).Compile()();      
      
      // Construct an action then branch depending on whether the member to be
      // interpolated is a property or a field
      Action action = null;
      //----------------------------------------------------------------------/
      // Property
      var property = targetObj.GetType().GetProperty(variableName);
      if (property != null)
      {
        // Type of the property?
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
      }
      //----------------------------------------------------------------------/
      // Field
      else
      {
        var field = targetObj.GetType().GetField(variableName);
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
      }      
      // Now add it!
      set.Add(action);
    }

  }

}

