/******************************************************************************/
/*!
@file   Interpolation.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace Utilities
  {
    /// <summary>
    /// A general-purpose utility class for interpolation of struct types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Interpolator<T>
    {
      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      protected T _StartingValue;
      protected T _CurrentValue;
      protected T _EndingValue;
      bool _Active;
      Stopwatch Timer;

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// The starting value
      /// </summary>
      public T StartingValue
      {
        set
        {
          _StartingValue = value;
        }
      }

      /// <summary>
      /// The current value
      /// </summary>
      public T CurrentValue
      {
        get
        {
          if (Active)
            return _CurrentValue;
          return _EndingValue;
        }
      }

      /// <summary>
      /// The ending value
      /// </summary>
      public T EndingValue
      {
        set
        {
          _EndingValue = value;
        }
      }

      /// <summary>
      /// Whether it is currently interpolating
      /// </summary>
      public bool Active { get { return _Active; } }

      //----------------------------------------------------------------------/
      // Configuration
      //----------------------------------------------------------------------/
      public Interpolator()
      {
      }


      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Begins interpolation
      /// </summary>
      /// <param name="time"></param>
      public void Start(float time)
      {
        Timer = new Stopwatch(time);
        _Active = true;
      }

      /// <summary>
      /// Updates the interpolator
      /// </summary>
      public void Update()
      {
        Update(Time.deltaTime);
      }

      /// <summary>
      /// Updates the interpolator
      /// </summary>
      /// <param name="dt"></param>
      public void Update(float dt)
      {
        if (Active)
        {
          if (Timer.Update(dt))
          {
            Timer.Reset();
            _Active = false;
          }

          float t = Timer.normalizedProgress;
          Interpolate(t);

          // Interpolate depending on type
          if (CurrentValue is Vector3)
          {
            
          }

        }
      }

      protected abstract void Interpolate(float t);
    }

    /// <summary>
    /// Interpolates a Vector3
    /// </summary>
    public class Vector3Interpolator : Interpolator<Vector3>
    {
      protected override void Interpolate(float t)
      {
        _CurrentValue = Vector3.Slerp(_StartingValue, _EndingValue, t);
      }
    }

    /// <summary>
    /// Interpolates a Vector2
    /// </summary>
    public class Vector2Interpolator : Interpolator<Vector2>
    {
      protected override void Interpolate(float t)
      {
        _CurrentValue = Vector2.Lerp(_StartingValue, _EndingValue, t);
      }
    }






  }
}

