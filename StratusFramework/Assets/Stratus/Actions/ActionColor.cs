/******************************************************************************/
/*!
@file   ActionColor.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionColor 
  */
  /**************************************************************************/
  public class ActionColorProperty<T> : ActionProperty where T :Renderer
  {
    T Property;
    Color CurrentValue;
    Color Difference;
    Color InitialValue;
    Color EndValue;

    public ActionColorProperty(T property, Color endValue, float duration, Ease ease) : base(duration, ease)
    {
      Property = property;
      EndValue = endValue;      
      Duration = duration;

      InitialValue = Property.material.color;
      CurrentValue = InitialValue;
      Difference = EndValue - InitialValue;

      
    }

    /**************************************************************************/
    /*!
    @brief Updates the ActionProperty.
    @param dt The time slice given.
    */
    /**************************************************************************/
    public override float Interpolate(float dt)
    {
      // Retrieve the initial value at the time it starts to get updated!
      if (this.Elapsed == 0.0f)
      {
        InitialValue = Property.material.color;
        CurrentValue = InitialValue;
        Difference = EndValue - InitialValue;
        //Debug.Log("InitialValue = " + InitialValue + ", Endvalue = " + EndValue);
      }

      this.Elapsed += dt;
      var timeLeft = this.Duration - this.Elapsed;
      float timeConsumed;
      

      // If there is no time left...
      if (timeLeft <= dt)
      {
        timeConsumed = dt;
        this.Finished = true;
        Property.material.color = this.EndValue;
      }
      else
      {
        //Debug.Log("Property = '" + Property.material.color + "', dt = '" + dt + "', timeLeft = '" + timeLeft + "'");
        var easeVal = Easing.Calculate((this.Elapsed / this.Duration), this.EaseType);
        Property.material.color = this.InitialValue + this.Difference * easeVal;
        timeConsumed = timeLeft;
        //Property.material.color = Color.Lerp(Property.material.color, this.EndValue, dt);
      }

      return timeConsumed;
    }

  }

}