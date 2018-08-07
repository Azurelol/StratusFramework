using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Dependencies.TypeReferences;
using Stratus.Dependencies.Ludiq.Reflection;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace Stratus.Gameplay
{
  /// <summary>
  /// Base class for animation hooks
  /// </summary>
  public abstract class AnimatorHook
  {
    /// <summary>
    /// The name of the parameter to use in animator
    /// </summary>
    [Tooltip("The name of the animator parameter whose value to set")]
    public string parameterName;
    /// <summary>
    /// The type of parameter being set (float, bool, int, trigger)
    /// </summary>
    public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;
  }

  /// <summary>
  /// Hooks a given Stratus Event to an animation
  /// </summary>
  [Serializable]
  public class AnimatorEventHook : AnimatorHook
  {
    [ClassExtends(typeof(Stratus.Event), Grouping = ClassGrouping.ByNamespace, AllowAbstract = false)]
    public ClassTypeReference onEvent;
    public AnimatorControllerParameter parameter;    
    public bool boolValue;
    public int intValue;
    public float floatValue;

    public void SetParameter(AnimatorControllerParameter parameter)
    {
      this.parameter = new AnimatorControllerParameter();
      this.parameter.defaultBool = parameter.defaultBool;
      this.parameter.defaultFloat = parameter.defaultFloat;
      this.parameter.defaultInt = parameter.defaultInt;
      this.parameter.type = parameter.type;
    }

    public void Play(CharacterAnimator ca)
    {
      switch (parameterType)
      {
        case AnimatorControllerParameterType.Float:
          ca.SetFloat(parameter.name, floatValue);
          break;
        case AnimatorControllerParameterType.Int:
          ca.SetInteger(parameter.name, intValue);
          break;
        case AnimatorControllerParameterType.Bool:
          ca.SetBoolean(parameter.name, boolValue);
          break;
        case AnimatorControllerParameterType.Trigger:
          ca.SetTrigger(parameter.name);
          break;
      }
    }
  }

  /// <summary>
  /// Updates the selected parameters with the given member
  /// </summary>
  [Serializable]
  public class AnimatorParameterHook : AnimatorHook
  {
    /// <summary>
    /// The member that will be used to update this parameter
    /// </summary>
    [Tooltip("The member whose value will be used")]
    [Filter(typeof(float), typeof(int), typeof(bool), Methods = false, Properties = true, NonPublic = true, ReadOnly = true, Static = true, Inherited = true, Fields = true)]
    public UnityMember member;
    /// <summary>
    /// Whether the member has been assigned to this hook
    /// </summary>
    public bool isAssigned => member.isAssigned;

    /// <summary>
    /// Updates a given parameter in the character animator
    /// </summary>
    /// <param name="characterAnimator"></param>
    public void Update(CharacterAnimator characterAnimator)
    {
      switch (parameterType)
      {
        case AnimatorControllerParameterType.Float:
          characterAnimator.SetFloat(parameterName, member.Get<float>());
          break;

        case AnimatorControllerParameterType.Int:
          characterAnimator.SetInteger(parameterName, member.Get<int>());
          break;

        case AnimatorControllerParameterType.Bool:
          characterAnimator.SetBoolean(parameterName, member.Get<bool>());
          break;
      }
    }

    /// <summary>
    /// Deduces the current parameter type for this hook
    /// </summary>
    public void DeduceParameterType()
    {
      if (member.type == typeof(int))
        parameterType = AnimatorControllerParameterType.Int;
      else if (member.type == typeof(float))
        parameterType = AnimatorControllerParameterType.Float;
      else if (member.type == typeof(bool))
        parameterType = AnimatorControllerParameterType.Bool;
    }

  }

  [Serializable]
  public class AnimationParameterEvent : UnityEvent<float, bool, int> { }

}