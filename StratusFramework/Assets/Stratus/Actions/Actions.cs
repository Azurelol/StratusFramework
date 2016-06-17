/******************************************************************************/
/*!
@file   Actions.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/22/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus {

  /**************************************************************************/
  /*!
  @class Actions Interface class that the client will be using for constructing 
         and interacting with actions.
  */
  /**************************************************************************/
  public static class Actions {
    public static bool Trace = false;

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
    @brief Creates an ActionProperty for a 'float' property.
    @param set A reference to the set.
    @param func The function to which to call.
    */
    /**************************************************************************/
    public static void Property(ActionSet set, Real property, float value, float duration, Ease ease)
    {      
      Action actionProperty = new ActionFloatProperty(property, value, duration, ease);
      set.Add(actionProperty);
    }

    /**************************************************************************/
    /*!
    @brief Creates an ActionProperty for a 'float' property.
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
    @brief Creates an ActionProperty for a 'float' property.
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
    @brief Creates an ActionProperty for a 'float' property.
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
    @brief Creates an ActionProperty for a 'int' property.
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
    @brief Creates an ActionProperty for a 'int' property.
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
    public static void Property<T>(ActionSet set, T renderer, Color value, float duration, Ease ease) where T : Renderer
    {
      //if (Trace)
      //  Debug.Log("");

      Action actionColor = new ActionColorProperty<T>(renderer, value, duration, ease);
      set.Add(actionColor);
    }

  }

}

