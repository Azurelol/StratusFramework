/******************************************************************************/
/*!
@file   ActionProperty.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;

namespace Stratus
{
  /// <summary>
  /// Common interpolation algorithms
  /// </summary>
  public enum Ease
  {
    /// <summary>
    /// Linear interpolation
    /// </summary>
    Linear,
    QuadIn,
    QuadInOut,
    QuadOut,
    SinIn,
    SinInOut,
    SinOut,
  }
  
  /// <summary>
  /// Provides methods for common interpolation algorithms.
  /// </summary>
  public static class Easing
  {
    public static float Linear(float t)
    {
      return t;
    }

    public static float QuadIn(float t)
    {
      return t * t;
    }

    public static float QuadOut(float t)
    {
      return t * (2 - t);
    }

    public static float Calculate(float t, Ease ease)
    {
      float easeVal = 0.0f;
      switch (ease)
      {
        case Ease.Linear:
          easeVal = Linear(t);
          break;
        case Ease.QuadIn:
          easeVal = QuadIn(t);
          break;
        case Ease.QuadOut:
          easeVal = QuadOut(t);
          break;

        case Ease.QuadInOut:
        case Ease.SinIn:
        case Ease.SinInOut:
        case Ease.SinOut:
          throw new System.NotImplementedException("That ease type is not supported yet!");
      }
      return easeVal;
    }

  }
  
  /// <summary>
  /// A type of action that modifies the value of
  /// a given property over a specified amount of time, using a specified
  /// interpolation formula(Ease).
  /// </summary>
  public abstract class ActionProperty : Action
  {
    protected Ease EaseType;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="duration">How long should the action delay for</param>
    /// <param name="ease">The interpolation algorithm to use</param>
    public ActionProperty(float duration, Ease ease) : base("ActionProperty")
    {
      this.duration = duration;
      EaseType = ease;
    }
    
    /// <summary>
    /// Updates the action
    /// </summary>
    /// <param name="dt">The delta time</param>
    /// <returns>How much time was consumed during this action</returns>
    public override float Update(float dt)
    {
      return this.Interpolate(dt);
    }

    public abstract float Interpolate(float dt);
  }

  /**************************************************************************/
  /*!
  @class ActionPropertyDelegate 
  */
  /**************************************************************************/
  public abstract class ActionPropertyGeneric<T> : ActionProperty
  {
    protected T Difference;
    protected T InitialValue;
    protected T EndValue;
    private bool Initialized = false;

    protected object Target;
    protected PropertyInfo Property;
    protected FieldInfo Field;

    /**************************************************************************/
    /*!
    @brief ActionPropertyGeneric constructor for Properties
    */
    /**************************************************************************/
    public ActionPropertyGeneric(object target, PropertyInfo property, T endValue, float duration, Ease ease)
  : base(duration, ease)
    {
      Target = target;
      Property = property;    
      EndValue = endValue;
      base.duration = duration;
      EaseType = ease;
    }

    /**************************************************************************/
    /*!
    @brief ActionPropertyGeneric constructor for Fields
    */
    /**************************************************************************/
    public ActionPropertyGeneric(object target, FieldInfo field, T endValue, float duration, Ease ease)
  : base(duration, ease)
    {
      Target = target;
      Field = field;   
      EndValue = endValue;
      base.duration = duration;
      EaseType = ease;
    }

    /**************************************************************************/
    /*!
    @brief  Interpolates the given Property/Field.
    @param  dt The current delta time.
    @return The time consumed.
    */
    /**************************************************************************/
    public override float Interpolate(float dt)
    {
      if (!Initialized)
        Initialize();

      this.elapsed += dt;
      var timeLeft = this.duration - this.elapsed;

      // If done updating
      if (timeLeft <= dt)
      {
        this.isFinished = true;
        this.SetLast();
        return dt;
      }

      this.SetCurrent();
      return timeLeft;
    }

    /**************************************************************************/
    /*!
    @brief Gets the initial value for the property. This is done separately
           because we want to capture the value at the time this action is beinig
           executed, not when it was created!
    */
    /**************************************************************************/
    public void Initialize()
    {
      if (Property != null)
        InitialValue = (T)Property.GetValue(Target, null);
      else if (Field != null)
        InitialValue = (T)Field.GetValue(Target);
      else
        throw new System.Exception("Couldn't set initial value!");

      // Now we can compute the difference
      ComputeDifference();

      if (Tracing) Trace.Script("InitialValue = '" + InitialValue 
                                + "', EndValue = '" + EndValue + "'"
                                + "' Difference = '" + Difference + "'");

      Initialized = true;
    }

    /**************************************************************************/
    /*!
    @brief Sets the current value for the property.
    */
    /**************************************************************************/
    public virtual void SetCurrent()
    {
      var easeVal = Easing.Calculate((this.elapsed / this.duration), this.EaseType);
      var currentValue = this.ComputeCurrentValue((easeVal));
      if (Tracing) Trace.Script("CurrentValue = '" + currentValue + "'");
      this.Set(currentValue);
    }

    /**************************************************************************/
    /*!
    @brief Sets the last value for the property.
    */
    /**************************************************************************/
    public virtual void SetLast()
    {
      this.Set(this.EndValue);
    }

    /**************************************************************************/
    /*!
    @brief Sets the value for the property.
    */
    /**************************************************************************/
    public void Set(T val)
    {
      if (this.Property != null)
      {
        Property.SetValue(Target, val, null);
      }
      else
      {
        Field.SetValue(Target, val);
      }
    }

    public abstract void ComputeDifference();
    public abstract T ComputeCurrentValue(float easeVal);
  }



}

