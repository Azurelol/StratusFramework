/******************************************************************************/
/*!
@file   ActionPropertyBoolean.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionBooleanProperty Provides interpolation for floats.
  */
  /**************************************************************************/
  public class ActionBooleanProperty : ActionProperty
  {
    Boolean.Setter Setter;
    Boolean.Getter Getter;
    Boolean InitialValue;
    Boolean EndValue;

    /**************************************************************************/
    /*!
    @brief ActionBooleanProperty constructor.
    @param property A reference to the property to be modified.
    @param value The new value to interpolate over the given duration.
    @param duration How long this property runs for.
    @param ease What ease this property uses to calculate the interpolation.
    */
    /**************************************************************************/
    public ActionBooleanProperty(Boolean property, bool value, float duration, Ease ease)
      : base(duration, ease)
    {
      this.Setter = property.Set; this.Getter = property.Get;
      this.InitialValue = property;
      this.EndValue = value;

      if (Actions.Debugging)
        Debug.Log("InitialValue = '" + this.InitialValue + "', EndValue = '" + this.EndValue + "'");
    }

    /**************************************************************************/
    /*!
    @brief Updates the ActionProperty.
    @param dt The time slice given.
    */
    /**************************************************************************/
    public override float Interpolate(float dt)
    {
      this.Elapsed += dt;

      var timeLeft = this.Duration - this.Elapsed;
      if (Actions.Debugging)
      {
        Debug.Log("Property = '" + this.Getter() + "', dt = '" + dt + "', timeLeft = '" + timeLeft + "'");
      }
      var timeConsumed = 0.0f;

      // If done updating
      if (timeLeft <= dt)
      {
        timeConsumed = dt;
        this.Finished = true;
        this.Setter(this.EndValue);
      }
      else
      {
        timeConsumed = timeLeft;
      }

      return timeConsumed;
    }
  }
}
