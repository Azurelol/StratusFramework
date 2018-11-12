using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Manages timed updates for behaviours.
  /// </summary>
  [Singleton(instantiate = true, isPlayOnly = true, persistent = true, name = "Stratus Update System")]
  public class StratusUpdateSystem : Singleton<StratusUpdateSystem>
  {
    //public struct UpdateSubscriber
    //{
    //  Dictionary<MonoBehaviour, List<System.Action>> behaviours;
    //}

    public class FrequencyUpdateBatch
    {
      /// <summary>
      /// How often to update these methods
      /// </summary>
      private Countdown timer;
      /// <summary>
      /// The behaviours which to be updated
      /// </summary>
      private Dictionary<MonoBehaviour, List<System.Action>> behaviours;
      /// <summary>
      /// How many actions are being updated
      /// </summary>
      public int methodCount { get; private set; }
      /// <summary>
      /// How many actions are being updated
      /// </summary>
      public int behaviourCount => behaviours.Count;

      public FrequencyUpdateBatch(float frequency)
      {
        behaviours = new Dictionary<MonoBehaviour, List<System.Action>>();
        timer = new Countdown(frequency);
        timer.SetCallback(Invoke);
        timer.resetOnFinished = true;
      }

      private void Invoke()
      {
        foreach (var behaviour in behaviours)
        {
          if (!behaviour.Key.enabled)
            continue;

          foreach (var action in behaviour.Value)
          {
            action.Invoke();
          }
        }
      }

      public void Add(System.Action action, MonoBehaviour behaviour)
      {
        if (!behaviours.ContainsKey(behaviour))
          behaviours.Add(behaviour, new List<System.Action>());

        behaviours[behaviour].Add(action);
        methodCount++;
      }

      public void Remove(MonoBehaviour behaviour)
      {
        if (!behaviours.ContainsKey(behaviour))
          return;

        methodCount -= behaviours[behaviour].Count;
        behaviours.Remove(behaviour);
      }

      public void Update(float time)
      {
        //Trace.Script($"time = {time}, progress = {timer.progress}");
        timer.Update(time);
      }

    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/

    private Dictionary<float, FrequencyUpdateBatch> update = new Dictionary<float, FrequencyUpdateBatch>();

    private Dictionary<float, FrequencyUpdateBatch> fixedUpdate = new Dictionary<float, FrequencyUpdateBatch>();
    /// <summary>
    /// A list of timers being updated
    /// </summary>
    private List<Timer> timers = new List<Timer>();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
    }

    private void Update()
    {
      foreach (var batch in update)
      {
        batch.Value.Update(Time.deltaTime);
      }

      foreach(var timer in this.timers)
      {
        if (!timer.isFinished)
          timer.Update(Time.deltaTime);
      }
    }

    private void FixedUpdate()
    {
      foreach (var batch in update)
      {
        batch.Value.Update(Time.fixedDeltaTime);
      }
    }

    //------------------------------------------------------------------------/
    // Static Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Subscribes a method to be invoked with the given frequency on the given timescale
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="action"></param>
    /// <param name="behaviour"></param>
    /// <param name="timeScale"></param>
    public static void Add(float frequency, System.Action action, MonoBehaviour behaviour, TimeScale timeScale = TimeScale.Delta)
    {
      instance.AddAction(frequency, action, behaviour, timeScale);
    }

    /// <summary>
    /// Adds a timer to be updated by the system
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="action"></param>
    /// <param name="behaviour"></param>
    /// <param name="timeScale"></param>
    public static void Add(Timer timer)
    {
      instance.timers.Add(timer);
    }

    /// <summary>
    /// Removes all subscribed methods for this behaviour on the given timescale
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="action"></param>
    /// <param name="behaviour"></param>
    /// <param name="timeScale"></param>
    public static void Remove(MonoBehaviour behaviour, TimeScale timeScale = TimeScale.Delta)
    {
      instance.RemoveBehaviour(behaviour, timeScale);
    }

    //------------------------------------------------------------------------/
    // Private Methods
    //------------------------------------------------------------------------/
    private void RemoveBehaviour(MonoBehaviour behaviour, TimeScale timeScale = TimeScale.Delta)
    {
      Dictionary<float, FrequencyUpdateBatch> selected = null;

      switch (timeScale)
      {
        case TimeScale.Delta: selected = update; break;
        case TimeScale.FixedDelta: selected = fixedUpdate; break;
      }

      foreach (var kp in selected)
      {
        kp.Value.Remove(behaviour);
      }
    }

    public void AddAction(float frequency, System.Action action, MonoBehaviour behaviour, TimeScale timeScale = TimeScale.Delta)
    {
      Dictionary<float, FrequencyUpdateBatch> selected = null;

      switch (timeScale)
      {
        case TimeScale.Delta: selected = update; break;
        case TimeScale.FixedDelta: selected = fixedUpdate; break;
      }

      if (!selected.ContainsKey(frequency))
        selected.Add(frequency, new FrequencyUpdateBatch(frequency));

      selected[frequency].Add(action, behaviour);
    }




  }



}

