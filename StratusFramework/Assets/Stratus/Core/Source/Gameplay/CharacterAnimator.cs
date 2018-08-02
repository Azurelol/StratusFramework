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
  /// A component for generically managing animations in a character
  /// </summary>
  [DisallowMultipleComponent]
  public abstract class CharacterAnimator : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Event Declarations
    //------------------------------------------------------------------------/
    public abstract class BaseAnimatorEvent : Stratus.Event
    {
      /// <summary>
      /// The name of the animation
      /// </summary>
      public string name;
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="name"></param>
      public BaseAnimatorEvent(string name) { this.name = name; }
    }

    public abstract class SetValueEvent<T> : BaseAnimatorEvent
    {
      public T value;

      public SetValueEvent(string name, T value) : base(name)
      {
        this.value = value;
      }
    }

    /// <summary>
    /// Sets a specific bool with a given value
    /// </summary>
    public class SetBoolEvent : SetValueEvent<bool>
    {
      public SetBoolEvent(string name, bool value) : base(name, value)
      {
      }
    }

    /// <summary>
    /// Sets a specific float with a given value
    /// </summary>
    public class SetFloatEvent : SetValueEvent<float>
    {
      public SetFloatEvent(string name, float value) : base(name, value)
      {
      }
    }

    /// <summary>
    /// Sets a specific int with a given value
    /// </summary>
    public class SetIntegerEvent : SetValueEvent<int>
    {
      public SetIntegerEvent(string name, int value) : base(name, value)
      {
      }
    }

    /// <summary>
    /// Triggers a specific animation to be played
    /// </summary>
    public class SetTriggerEvent : BaseAnimatorEvent
    {
      public SetTriggerEvent(string name) : base(name) { }
    }

    /// <summary>
    /// Hooks a given Stratus Event to an animation
    /// </summary>
    [Serializable]
    public class AnimatorEventHook
    {
      [ClassExtends(typeof(Stratus.Event), Grouping = ClassGrouping.ByNamespace, AllowAbstract = false)]
      public ClassTypeReference onEvent;
      public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;
      public AnimatorControllerParameter parameter;
      public string parameterName;
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
    public class AnimatorParameterHook
    {
      /// <summary>
      /// The member that will be used to update this parameter
      /// </summary>
      [Tooltip("The member whose value will be used")]
      [Filter(typeof(float), typeof(int), typeof(bool), Methods = false, Properties = true, NonPublic = true, ReadOnly = true, Static = true, Inherited = true, Fields = true)]      
      public UnityMember member;
      /// <summary>
      /// The name of the parameter to use in animator
      /// </summary>
      [Tooltip("The name of the animator parameter whose value to set")]
      public string parameterName;

      /// <summary>
      /// The type of parameter
      /// </summary>
      public AnimatorControllerParameterType parameterType { get; set; }

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
    public class AnimationParameterEvent : UnityEvent<float, bool, int> {}

    public delegate void OnUpdate();

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/    
    public Animator animator;
    public Transform rootTransform;
    public List<AnimatorEventHook> animatorTriggers = new List<AnimatorEventHook>();
    public List<AnimatorParameterHook> animatorParameters = new List<AnimatorParameterHook>();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Returns the currently playing animation state
    /// </summary>
    protected AnimatorStateInfo currentState { get { return animator.GetCurrentAnimatorStateInfo(0); } }
    /// <summary>
    /// The trigger last played
    /// </summary>
    public string previousTrigger { get; private set; }
    /// <summary>
    /// Whether to print debug output
    /// </summary>
    public virtual bool debug { get; }
    protected OnUpdate onUpdate { get; set; }
    private Dictionary<Type, string> animatorEventTriggers = new Dictionary<Type, string>();

    //--------------------------------------------------------------------------------------------/
    // Virtual
    //--------------------------------------------------------------------------------------------/
    //protected abstract void OnUpdate();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      onUpdate = new OnUpdate(()=> { });
      SetHooks();
      Subscribe();
    }

    private void FixedUpdate()
    {
      UpdateParameters();
      onUpdate();
      //OnUpdate();
    }

    private void Reset()
    {
      this.animator = GetComponentInChildren<Animator>();
      if (animator)
        this.rootTransform = animator.transform;
    }

    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Subscribes to common events.
    /// </summary>
    void Subscribe()
    {
      this.gameObject.Connect<SetTriggerEvent>(this.OnSetTriggerEvent);
      this.gameObject.Connect<SetBoolEvent>(this.OnSetBoolEvent);
      this.gameObject.Connect<SetFloatEvent>(this.OnSetFloatEvent);
      this.gameObject.Connect<SetIntegerEvent>(this.OnSetIntegerEvent);
    }

    void OnSetFloatEvent(SetFloatEvent e) => SetFloat(e.name, e.value);
    void OnSetIntegerEvent(SetIntegerEvent e) => SetInteger(e.name, e.value);
    void OnSetBoolEvent(SetBoolEvent e) => SetBoolean(e.name, e.value);
    void OnSetTriggerEvent(SetTriggerEvent e) => SetTrigger(e.name);
    void OnEvent(Stratus.Event e)
    {
      Type eventType = e.GetType();
      if (!animatorEventTriggers.ContainsKey(eventType))
        return;

      string parameter = animatorEventTriggers[eventType];
      SetTrigger(parameter);
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Sets a boolean value on the animator
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected bool SetBoolean(string name, bool value)
    {
      if (!isActiveAndEnabled)
        return false;

      if (debug)
        Trace.Script("Setting boolean '" + name + "' to " + value, this);

      this.animator.SetBool(name, value);
      return true;
    }

    /// <summary>
    /// Activates a trigger on the animator
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool SetTrigger(string name)
    {
      if (!isActiveAndEnabled)
        return false;

      if (debug)
        Trace.Script("Setting trigger '" + name + "'", this);

      this.animator.SetTrigger(name);
      this.previousTrigger = name;
      return true;
    }

    /// <summary>
    /// Sets a float value on the animator
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected bool SetFloat(string name, float value)
    {
      if (!isActiveAndEnabled)
        return false;

      this.animator.SetFloat(name, value);
      return true;
    }

    /// <summary>
    /// Sets an integer value on the animator
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected bool SetInteger(string name, int value)
    {
      if (!isActiveAndEnabled)
        return false;

      this.animator.SetInteger(name, value);
      return true;
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Private
    //--------------------------------------------------------------------------------------------/
    protected void Lookat(Transform target, float trackingSpeed)
    {
      Vector3 lookAtVec = target.transform.position - this.transform.position;
      animator.transform.forward = Vector3.Lerp(this.transform.forward, lookAtVec, Time.deltaTime * (trackingSpeed * 2f));
    }

    public void FaceDirection(Vector3 direction, float speed)
    {
      rootTransform.forward = Vector3.Lerp(rootTransform.forward, direction, Time.deltaTime * (speed));
    }

    private void SetHooks()
    {
      foreach(var hook in animatorTriggers)
      {
        animatorEventTriggers.Add(hook.onEvent, hook.parameterName);
        gameObject.Connect(OnEvent, hook.onEvent);
        
      }

      foreach (var hook in animatorParameters)
      {
        hook.DeduceParameterType();
      }
    }

    private void UpdateParameters()
    {
      foreach (var parameter in animatorParameters)
      {
        if (parameter.isAssigned)
          parameter.Update(this);
      }
    }






  }

}