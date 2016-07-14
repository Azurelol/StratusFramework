/******************************************************************************/
/*!
@file   ActionPropertyInteger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  public class ActionPropertyInteger : ActionProperty
  {
    Integer.Setter Property;
    Integer.Getter Getter;
    float InitialValue;
    int EndValue;
    float CurrentValue;
    float Difference;

    /**************************************************************************/
    /*!
    @brief ActionFloatProperty constructor.
    @param property A reference to the property to be modified.
    @param value The new value to interpolate over the given duration.
    @param duration How long this property runs for.
    @param ease What ease this property uses to calculate the interpolation.
    */
    /**************************************************************************/
    public ActionPropertyInteger(Integer property, Integer value, Real duration, Ease ease)
      : base(duration, ease)
    {
      this.Property = property.Set; this.Getter = property.Get;
      this.InitialValue = property; this.EndValue = value;
      this.Difference = EndValue - InitialValue;

      if (Actions.Debugging)
        Debug.Log("InitialValue = '" + this.InitialValue + "', EndValue = '" + this.EndValue + "'");
    }

    /**************************************************************************/
    /*!
    @brief Updates the ActionProperty.
    @param dt The time slice given.
    @note I think the setting of the current value could be optimized...
    */
    /**************************************************************************/
    public override float Interpolate(float dt)
    {
      this.Elapsed += dt;

      var timeLeft = this.Duration - this.Elapsed;

      if (Actions.Debugging)
        Debug.Log("Property = '" + this.Getter() + "', dt = '" + dt + "', timeLeft = '" + timeLeft + "'");
      
      var timeConsumed = 0.0f;

      // If done updating
      if (timeLeft <= dt)
      {
        timeConsumed = dt;
        this.Finished = true;
        this.Property(this.EndValue);
      }
      else
      {
        // Calculate the current interpolated value
        var ease = Easing.Calculate((this.Elapsed / this.Duration), this.EaseType);
        this.CurrentValue = this.InitialValue + this.Difference * ease;        
        // Set it 
        this.Property(Mathf.CeilToInt(this.CurrentValue));
        timeConsumed = timeLeft;
      }


      return timeConsumed;
    }
  }

}

