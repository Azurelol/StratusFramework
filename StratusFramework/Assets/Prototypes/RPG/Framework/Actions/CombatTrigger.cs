/******************************************************************************/
/*!
@file   CombatTrigger.cs
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
using Stratus.Utilities;

namespace Prototype 
{
  [Serializable]
  public class CombatTrigger : ICloneable
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public enum Result { Critical, Success, Failure }
    public enum Type { Attack, Defend }
    public abstract class TriggerEvent : Stratus.Event
    {
      public Instance Instance;
    }
    public class StartedEvent : TriggerEvent { }
    public class EndedEvent : TriggerEvent
    {
      public Result Result;
      public float Percentage;
    }
    //------------------------------------------------------------------------/

    /// <summary>
    /// Represents a trigger input.
    /// </summary>
    [Serializable]
    public class Input : ICloneable
    {
      public enum InputState { Active, Ready, Inactive }

      /// <summary>
      /// The current state of this input
      /// </summary>
      InputState State = InputState.Inactive;
      ///// <summary>
      ///// What button is used for this input
      ///// </summary>
      //public Bindings Button = Bindings.Up;
      /// <summary>
      /// How many times this Trigger needs to be pressed before its successful.
      /// </summary>
      public Counter Presses = new Counter();   
      /// <summary>
      /// At what time is this input active
      /// </summary>
      [Tooltip("From what time to what time is this input active")]
      public Vector2 Range = new Vector2();
      /// <summary>
      /// When in the trigger timeline will this input be starting from.
      /// </summary>
      public float StartingTime { get { return Range.x; } }
      /// /// <summary>
      /// How long this input runs for
      /// </summary>
      public float Duration { get { return Range.y - Range.x; } }
      /// <summary>
      /// Checks whether this Input is active at this time / receiving input at this time.
      /// </summary>
      /// <param name="currenTime">The current time along the trigger</param>
      /// <returns>True if active, false otherwise.</returns>
      public void Update(float currentTime)
      {
        if (Range.Contains(currentTime))
        {
          if (State != InputState.Active)
            State = InputState.Ready;          
        }
        else
        {
          State = InputState.Inactive;
        }
        
      }

      //public bool Press(Bindings button)
      //{
      //  if (Button == button)
      //  {
      //    // If it hasn't already been completed...
      //    if (!Presses.isFull)
      //    {
      //      var full = Presses.Increment();
      //      //Trace.Script("Presses at '" + Presses.Print + "'");
      //      if (full)     
      //        return true;
      //    }
      //  }
      //
      //  return false;
      //}

      object ICloneable.Clone()
      {
        var clone = new Input();
        //clone.Button = Button;
        clone.Presses = new Counter(Presses.total);
        clone.Range = Range;
        return clone;
      }
    }

    /// <summary>
    /// An instantation of this trigger at runtime
    /// </summary>
    [Serializable]
    public class Instance
    {
      /// <summary>
      /// The trigger to set
      /// </summary>
      public CombatTrigger Trigger;
      /// <summary>
      /// The character this trigger is for
      /// </summary>
      public CombatController Caster;
      /// <summary>
      /// How long the trigger runs.
      /// </summary>
      public float Duration;
      /// <summary>
      /// The type of this trigger.
      /// </summary>
      public Type Type;
      /// <summary>
      /// Time elapsed since this trigger began running.
      /// </summary>
      public float Elapsed;
      /// <summary>
      /// How many successful presses have been made
      /// </summary>
      public int Pressed
      {
        get
        {
          int pressed = 0;
          foreach(var input in Trigger.Inputs)
          {
            pressed += input.Presses.current;            
          }
          return pressed;
        }
      }
      /// <summary>
      /// The percentage of successful presses made
      /// </summary>
      public float Percentage
      {
        get
        {
          //Trace.Script(Pressed.ToString() + "/" +  Trigger.Presses.ToString());
          return ((float)Pressed / (float)Trigger.Presses) * 100.0f;
        }
      }

      //----------------------------------------------------------------------/
      // Constructor
      //----------------------------------------------------------------------/
      public Instance(CombatTrigger trigger, CombatController caster, Type type, float duration)
      {
        Trigger = trigger;
        Caster = caster;
        Duration = duration;
        Elapsed = 0.0f;
        //Trace.Script("Caster = " + caster.Name + ", Duration = " + Duration, caster);
      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Updates this trigger. It will return true once the trigger has finished
      /// running.
      /// </summary>
      /// <returns>True if the trigger is done running, false otherwise. </returns>
      public bool Update()
      {
        Elapsed += Time.deltaTime;
        //Trace.Script(Elapsed + " / " + Duration);

        // If the trigger has run its duration...
        if (Elapsed >= Duration)
        {
          return true;
        }

        // Check every input
        foreach(var input in Trigger.Inputs)
        {
          input.Update(Elapsed);
        }

        return false;
      }
      
      //void Press(Bindings button)
      //{
      //  foreach(var input in Trigger.Inputs)
      //  {
      //    input.Press(button);
      //  }
      //}
      
         
    }


    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The list of inputs that have been configured for this trigger.
    /// </summary>
    public List<Input> Inputs = new List<Input>();
    /// <summary>
    ///  How many presses are needed in total for the trigger
    /// </summary>
    public int Presses
    {
      get
      {
        int hits = 0;
        foreach (var input in Inputs)
          hits += input.Presses.total;
        return hits;
      }
    }
    /// <summary>
    /// What outline to use for this trigger
    /// </summary>
    public GameObject Outline;




    //public ActionTrigger Trigger;

    /// <summary>
    /// Whether this trigger is enabled.
    /// </summary>
    public bool Enabled { get { if (Inputs.Count > 0) return true; return false; } }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public CombatTrigger Copy(CombatTrigger rhs)
    {
      var trigger = new CombatTrigger();
      trigger.Inputs = rhs.Inputs.Clone();
      return trigger;      
    }

    object ICloneable.Clone()
    {
      var trigger = new CombatTrigger();
      trigger.Inputs = Inputs.Clone();
      return trigger;
    }


    /// <summary>
    /// Starts this trigger.
    /// </summary>
    /// <param name="space"></param>
    public void Start(CombatController caster, Type type, float duration)
    {
      var startedEvent = new StartedEvent();
      // Make a copy of this trigger ??
      startedEvent.Instance = new CombatTrigger.Instance(Copy(this), caster, type, duration);
      Scene.Dispatch<StartedEvent>(startedEvent);
    }

    /// <summary>
    /// The modifier applied by the result of the Trigger.
    /// </summary>
    /// <param name="result">The result</param>
    /// <returns>A modifying multiplier applied to an effect.</returns>
    public static float Modifier(Result result)
    {
      if (result == Result.Critical)
        return 2.0f;
      else if (result == Result.Success)
        return 1.0f;

      return 0.0f;
    }
    


  }
}
