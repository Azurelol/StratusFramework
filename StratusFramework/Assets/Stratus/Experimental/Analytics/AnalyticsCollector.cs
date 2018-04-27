using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.TypeReferences;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

namespace Stratus.Analytics
{
  //[ExecuteInEditMode]
  public class AnalyticsCollector : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Declaratioins
    //------------------------------------------------------------------------/


    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public bool debug;
    public AnalyticsPayload target;

    // Conditions: When the data should be collected
    [Header("Condition")]
    public Condition condition;
    // Event
    [Tooltip("The scope of the event")]
    public Event.Scope eventScope;
    [ClassExtends(typeof(Stratus.Event), Grouping = ClassGrouping.ByNamespace)]
    [Tooltip("What type of event will have this data be collected")]
    public ClassTypeReference eventType;
    // Timer
    [Tooltip("How often the members are polled for their current value")]
    [Range(0f, 3f)]
    public float onTimer = 1f;
    // Messages
    public MessageType onMessage;
    public TriggerLifecycleEvent onLifecycleEvent;
    public EventTriggerType onEventTriggerType;

    //[Header("Rules")]
    //public bool useRules;
    //[Tooltip("The maximum amount of times ")]
    //public int repetitions = 0;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public UnityEngine.EventSystems.EventTrigger eventTrigger { get; private set; }
    public EventProxy eventProxy { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      //if (Application.isPlaying)
      
      switch (condition)
      {
        case Condition.Timer:
          UpdateSystem.Add(onTimer, Submit, this);
          break;

        case Condition.Nessage:
          switch (onMessage)
          {
            case MessageType.LifecycleEvent:
              // proxy here?
              break;
            case MessageType.EventTriggerType:
              //eventTrigger = this.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
              //eventTrigger.triggers.Add(new UnityEngine.EventSystems.EventTrigger.Entry())
              break;
          }
          break;

        case Condition.Event:
          eventProxy = EventProxy.Construct(this.gameObject, eventScope, eventType, OnEvent, true, debug);
          break;
        default:
          break;
      }
    }

    private void OnDestroy()
    {
      
      switch (condition)
      {
        case Condition.Timer:
          UpdateSystem.Remove(this);
          break;
        case Condition.Nessage:
          break;
        case Condition.Event:
          Destroy(eventProxy);
          break;
        default:
          break;
      }
    }

    private void OnEnable()
    {
      AnalyticsEngine.Connect(this);
    }

    private void OnDisable()
    {
      AnalyticsEngine.Disconnect(this);
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    void OnEvent<T>(T e) where T : Stratus.Event
    {
      Submit();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Submits the current data point of the target
    /// </summary>
    public void Submit()
    {
      if (target.hasValue)

      Trace.Script("Collecting!", this);
    }

  }

}