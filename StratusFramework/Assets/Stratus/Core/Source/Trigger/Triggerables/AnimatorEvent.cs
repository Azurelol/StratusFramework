/******************************************************************************/
/*!
@file   AnimateEvent.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;
using UnityEngine.Serialization;

namespace Stratus 
{
  public class AnimatorEvent : Triggerable
  {
    [FormerlySerializedAs("Animator")]
    public Animator animator;

    [Header("Parameters")]
    public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;
    [FormerlySerializedAs("Animation")]
    public string identifier;
    [DrawIf(nameof(parameterType), AnimatorControllerParameterType.Bool, ComparisonType.Equals)]
    public bool booleanValue;
    [DrawIf(nameof(parameterType), AnimatorControllerParameterType.Float, ComparisonType.Equals)]
    public float floatValue;
    [DrawIf(nameof(parameterType), AnimatorControllerParameterType.Int, ComparisonType.Equals)]
    public int integerValue;
    

    protected override void OnAwake()
    {
      
    }

    protected override void OnReset()
    {
      
    }

    protected override void OnTrigger()
    {
      switch (parameterType)
      {
        case AnimatorControllerParameterType.Float:
          animator.SetFloat(this.identifier, this.floatValue);
          break;
        case AnimatorControllerParameterType.Int:
          animator.SetInteger(this.identifier, this.integerValue);
          break;
        case AnimatorControllerParameterType.Bool:
          animator.SetBool(this.identifier, this.booleanValue);   
          break;
        case AnimatorControllerParameterType.Trigger:
          animator.SetTrigger(this.identifier);
          break;
      }     

    }
  }
}
