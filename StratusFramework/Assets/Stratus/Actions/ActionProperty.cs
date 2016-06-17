/******************************************************************************/
/*!
@file   ActionProperty.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/22/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class Ease Common interpolation algorithms.
  /**************************************************************************/
  public enum Ease
  {
    Linear,
    QuadIn,
    QuadInOut,
    QuadOut,
    SinIn,
    SinInOut,
    SinOut,
  }

  /**************************************************************************/
  /*!
  @class Easing provides methods for common interpolation algorithms.
  */
  /**************************************************************************/
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
          easeVal = Linear(t); break;
        case Ease.QuadIn:
          easeVal = QuadIn(t); break;
        case Ease.QuadOut:
          easeVal = QuadOut(t); break;
      }
      return easeVal;
    }

  }

  /**************************************************************************/
  /*!
  @class ActionProperty A type of action that modifies the value of
         a given property over a specified amount of time, using a specified
         interpolation formula (Ease).
  */
  /**************************************************************************/
  public abstract class ActionProperty : Action
  {
    public static bool Trace = true;

    protected Ease EaseType;

    /**************************************************************************/
    /*!
    @brief ActionProperty constructor
    @param duration How long this Action should delay for.
    */
    /**************************************************************************/
    public ActionProperty(float duration, Ease ease) : base("ActionProperty")
    {
      this.Duration = duration;
      EaseType = ease;
    }

    /**************************************************************************/
    /*!
    @brief Updates the action
    @param dt The delta time.
    @return How much time was consumed during this action.
    */
    /**************************************************************************/
    public override float Update(float dt)
    {
      return this.Interpolate(dt);
    }

    public abstract float Interpolate(float dt);
  }

  

}

