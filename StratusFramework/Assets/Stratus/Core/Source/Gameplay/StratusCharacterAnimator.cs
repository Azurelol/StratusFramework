using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Dependencies.TypeReferences;
using Stratus.Dependencies.Ludiq.Reflection;
using UnityEngine.Animations;
using UnityEngine.Events;
using System.Linq;

namespace Stratus.Gameplay
{
  /// <summary>
  /// A component for generically managing animations in a character
  /// </summary>
  [DisallowMultipleComponent]
  public abstract class StratusCharacterAnimator : ManagedBehaviour
  {
    //------------------------------------------------------------------------/
    // Event Declarations
    //------------------------------------------------------------------------/
    public abstract class BaseAnimatorEvent : Stratus.StratusEvent
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

    public delegate void UpdateFunction();

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/    
    public Animator animator;
    public Transform rootTransform;
    [Tooltip("When the specified event is received by this GameObject, set the given values onto the animator")]
    public List<StratusAnimatorEventHook> animatorEventHooks = new List<StratusAnimatorEventHook>();
    [Tooltip("When the specified member changes, update the corresponding parameter on the animator")]
    public List<StratusAnimatorParameterHook> animatorParameterHooks = new List<StratusAnimatorParameterHook>();
    [SerializeField]
    private AnimatorControllerParameter[] _animatorParameters;
    [SerializeField]
    private string [] _animatorParameterNames;
    [SerializeField]
    private string[] _floatParameters, _boolParameters, _intParameters, _triggerParameters;

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
    /// <summary>
    /// Whether there's available parameters for the character animator
    /// </summary>
    public bool hasParameters => _animatorParameters != null && _animatorParameters.NotEmpty();

    // - Parameter names
    public string[] animatorParameterNames => this._animatorParameterNames;
    public string[] floatParameters => this._floatParameters;
    public string[] intParameters => this._intParameters;
    public string[] boolParameters => this._boolParameters;
    public string[] triggerParameters => this._triggerParameters;

    protected UpdateFunction onUpdate { get; set; }

    private Dictionary<Type, string> animatorEventTriggers = new Dictionary<Type, string>();

    //--------------------------------------------------------------------------------------------/
    // Virtual
    //--------------------------------------------------------------------------------------------/
    //protected abstract void OnUpdate();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnManagedAwake()
    {
      onUpdate = new UpdateFunction(() => { });
      SetHooks();
      Subscribe();
    }

    protected override void OnManagedFixedUpdate()
    {
      UpdateParameters();
      onUpdate();
    }

    private void Reset()
    {
      this.animator = GetComponentInChildren<Animator>();
      if (this.animator)
      {
        this.rootTransform = animator.transform;
        this.RecordParameters();
      }
    }

    private void OnValidate()
    {
      this.RecordParameters();      
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
    void OnEvent(Stratus.StratusEvent e)
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
    public bool SetBoolean(string name, bool value)
    {
      if (!isActiveAndEnabled)
        return false;

      if (debug)
        StratusDebug.Log("Setting boolean '" + name + "' to " + value, this);

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
        StratusDebug.Log("Setting trigger '" + name + "'", this);

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
    public bool SetFloat(string name, float value)
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
    public bool SetInteger(string name, int value)
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
      foreach(var hook in animatorEventHooks)
      {
        if (hook.onEvent.Type == null || hook.parameterName == null)
        {
          StratusDebug.Error($"An animator trigger has not been set properly!", this);
          continue;
        }
        animatorEventTriggers.Add(hook.onEvent, hook.parameterName);
        gameObject.Connect(OnEvent, hook.onEvent);
        
      }

      foreach (var hook in animatorParameterHooks)
      {
        if (!hook.member.isAssigned)
        {
          StratusDebug.Error($"An animator parameter has not been set properly!", this);
          continue;
        }

        hook.DeduceParameterType();
      }
    }

    private void UpdateParameters()
    {
      foreach (var parameter in animatorParameterHooks)
      {
        if (parameter.isAssigned)
          parameter.Update(this);
      }
    }

    private void RecordParameters()
    {
      foreach(var hook in this.animatorParameterHooks)
      {
        if( hook.member.isAssigned)
          hook.DeduceParameterType();
      }

      if (this.animator)
      {
        this._animatorParameters = this.animator.parameters;
        this._animatorParameterNames = this._animatorParameters.Names((AnimatorControllerParameter parameter) => parameter.name);
        this._floatParameters = this._animatorParameters.Where(x => x.type == AnimatorControllerParameterType.Float).ToArray().Names((AnimatorControllerParameter parameter) => parameter.name);
        this._intParameters = this._animatorParameters.Where(x => x.type == AnimatorControllerParameterType.Int).ToArray().Names((AnimatorControllerParameter parameter) => parameter.name);
        this._boolParameters = this._animatorParameters.Where(x => x.type == AnimatorControllerParameterType.Bool).ToArray().Names((AnimatorControllerParameter parameter) => parameter.name);
        this._triggerParameters = this._animatorParameters.Where(x => x.type == AnimatorControllerParameterType.Trigger).ToArray().Names((AnimatorControllerParameter parameter) => parameter.name);
      } 
      else
      {
        this._animatorParameters = null;
        this._animatorParameterNames = this._floatParameters = this._intParameters = this._triggerParameters = null;
      }
    }






  }

}