using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using System.Linq;

namespace Stratus
{
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  public class TriggerSystem : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public List<Trigger> triggers = new List<Trigger>();
    public List<Triggerable> triggerables = new List<Triggerable>();
    public bool showDescriptions = false;
    public bool descriptionsWithLabel = false;
    private Dictionary<Trigger, bool> triggersInitialState = new Dictionary<Trigger, bool>();
    private Dictionary<Triggerable, bool> triggerablesInitialState = new Dictionary<Triggerable, bool>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether there no components in the system
    /// </summary>    
    public bool isEmpty => triggers.Empty() && triggerables.Empty();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      RecordTriggerStates();
    }

    private void OnDestroy()
    {
      ShowComponents(true);
    }

    private void OnEnable()
    {
      Refresh();
    }

    private void Reset()
    {
      Refresh();
    }

    private void OnValidate()
    {
      ShowComponents(false);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void Initialize()
    {
      //Trace.Script("Initializing");
      
    }

    private void RecordTriggerStates()
    {
      foreach (var trigger in triggers)
        triggersInitialState.Add(trigger, trigger.enabled);

      foreach (var triggerable in triggerables)
        triggerablesInitialState.Add(triggerable, triggerable.enabled);
    }

    /// <summary>
    /// Restars the state of all the triggers in this system to their intitial values
    /// </summary>
    public void Restart()
    {
      foreach (var trigger in triggers)
        trigger.Restart();
      //trigger.enabled = triggersInitialState[trigger];

      foreach (var triggerable in triggerables)
        triggerable.Restart();
        //triggerable.enabled = true;
        //triggerable.enabled = triggerablesInitialState[triggerable];
    }

    /// <summary>
    /// Toggles all triggers in the system on/off
    /// </summary>
    public void Toggle(bool toggle)
    {
      foreach (var trigger in triggers)
      {
        // Skip triggers that were marked as not persistent and have been activated
        if (!trigger.persistent && trigger.activated)
          continue;
        //Trace.Script($"Toggling {trigger.GetType().Name} to {toggle}", this);
        trigger.enabled = toggle;
      }
    }



    /// <summary>
    /// Adds a trigger to the system
    /// </summary>
    /// <param name="baseTrigger"></param>
    public void Add(TriggerBase baseTrigger)
    {
      if (baseTrigger is Trigger)
        triggers.Add(baseTrigger as Trigger);
      else if (baseTrigger is Triggerable)
        triggerables.Add(baseTrigger as Triggerable);
    }

    /// <summary>
    /// Controls visibility for all the base trigger components
    /// </summary>
    /// <param name="show"></param>
    public void ShowComponents(bool show)
    {
      HideFlags flag = show ? HideFlags.None : HideFlags.HideInInspector;
      foreach (var trigger in triggers)
        trigger.hideFlags = flag;
      foreach (var triggerable in triggerables)
        triggerable.hideFlags = flag;
    }
    
    /// <summary>
    /// Refreshes the state of this TriggerSystem
    /// </summary>
    private void Refresh()
    {
      // Remove any invalid
      triggers.RemoveNull();
      triggerables.RemoveNull();

      // Add previously not found
      triggers.AddRangeUnique(GetComponents<Trigger>());
      triggerables.AddRangeUnique(GetComponents<Triggerable>());

      // Hide any triggers managed by the system
      ShowComponents(false);

      // Validate triggers
      ValidateTriggers();
    }
    
    private void ValidateTriggers()
    {
      foreach (var trigger in triggers)
      {
        trigger.scope = Trigger.Scope.Component;
      }        
    }
    

  }

}