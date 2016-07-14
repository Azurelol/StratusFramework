/******************************************************************************/
/*!
@file   ActionPropertyReal.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;


namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionFloatProperty Provides interpolation for floats.
  */
  /**************************************************************************/
  public class ActionPropertyReal : ActionProperty
  {
    Real.Setter Setter;
    Real.Getter Getter;
    float Difference;
    float InitialValue;
    float EndValue;

    /**************************************************************************/
    /*!
    @brief ActionFloatProperty constructor.
    @param property A reference to the property to be modified.
    @param value The new value to interpolate over the given duration.
    @param duration How long this property runs for.
    @param ease What ease this property uses to calculate the interpolation.
    */
    /**************************************************************************/
    public ActionPropertyReal(Real property, float value, float duration, Ease ease)
      : base(duration, ease)
    {
      this.Setter = property.Set; this.Getter = property.Get;
      this.InitialValue = property; this.EndValue = value;
      this.Difference = EndValue - InitialValue;

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
        var easingVal = Easing.Calculate((this.Elapsed / this.Duration), this.EaseType);
        this.Setter(this.InitialValue + this.Difference * easingVal);
      }


      return timeConsumed;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionFloatProperty Provides interpolation for floats.
  */
  /**************************************************************************/
  public class ActionReal2Property : ActionProperty
  {
    Real2.Setter Setter;
    Real2.Getter Getter;
    Vector2 Difference;
    Vector2 InitialValue;
    Vector2 EndValue;

    /**************************************************************************/
    /*!
    @brief ActionFloatProperty constructor.
    @param property A reference to the property to be modified.
    @param value The new value to interpolate over the given duration.
    @param duration How long this property runs for.
    @param ease What ease this property uses to calculate the interpolation.
    */
    /**************************************************************************/
    public ActionReal2Property(Real2 property, Vector2 value, float duration, Ease ease)
      : base(duration, ease)
    {
      this.Setter = property.Set; this.Getter = property.Get;
      this.InitialValue = property; this.EndValue = value;
      this.Difference = EndValue - InitialValue;

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
        var easingVal = Easing.Calculate((this.Elapsed / this.Duration), this.EaseType);
        this.Setter(this.InitialValue + this.Difference * easingVal);
      }


      return timeConsumed;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionFloatProperty Provides interpolation for floats.
  */
  /**************************************************************************/
  public class ActionReal3Property : ActionProperty
  {
    Real3.Setter Setter;
    Real3.Getter Getter;
    Vector3 Difference;
    Vector3 InitialValue;
    Vector3 EndValue;

    /**************************************************************************/
    /*!
    @brief ActionFloatProperty constructor.
    @param property A reference to the property to be modified.
    @param value The new value to interpolate over the given duration.
    @param duration How long this property runs for.
    @param ease What ease this property uses to calculate the interpolation.
    */
    /**************************************************************************/
    public ActionReal3Property(Real3 property, Vector3 value, float duration, Ease ease)
      : base(duration, ease)
    {
      this.Setter = property.Set;
      this.Getter = property.Get;
      this.EndValue = value;

      this.InitialValue = property;
      this.Difference = EndValue - InitialValue;

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
        var easingVal = Easing.Calculate((this.Elapsed / this.Duration), this.EaseType);
        this.Setter(this.InitialValue + this.Difference * easingVal);
      }


      return timeConsumed;
    }
  }

  /**************************************************************************/
  /*!
  @class ActionFloatProperty Provides interpolation for floats.
  */
  /**************************************************************************/
  public class ActionReal4Property : ActionProperty
  {
    Real4.Setter Setter;
    Real4.Getter Getter;
    Vector4 Difference;
    Vector4 InitialValue;
    Vector4 EndValue;

    /**************************************************************************/
    /*!
    @brief ActionFloatProperty constructor.
    @param property A reference to the property to be modified.
    @param value The new value to interpolate over the given duration.
    @param duration How long this property runs for.
    @param ease What ease this property uses to calculate the interpolation.
    */
    /**************************************************************************/
    public ActionReal4Property(Real4 property, Vector4 value, float duration, Ease ease)
      : base(duration, ease)
    {
      this.Setter = property.Set;
      this.Getter = property.Get;
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
      var timeConsumed = 0.0f;

      if (Actions.Debugging)
        Debug.Log("Property = '" + this.Getter() + "', dt = '" + dt + "', timeLeft = '" + timeLeft + "'");

      // Retrieve the initial value at the time it starts to get updated!
      if (this.Elapsed < 0)
      {
        this.InitialValue = this.Getter();
        this.Difference = EndValue - InitialValue;
      }

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
        var easingVal = Easing.Calculate((this.Elapsed / this.Duration), this.EaseType);
        this.Setter(this.InitialValue + this.Difference * easingVal);
      }


      return timeConsumed;
    }
  }

}


