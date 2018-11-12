using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
  public partial class TriggerSystemEditor : StratusBehaviourEditor<StratusTriggerSystem>
  {
    /// <summary>
    /// Validates all the components within the system
    /// </summary>
    private void ValidateAll()
    {
      var messages = ObjectValidation.Aggregate(target);
      foreach (var msg in messages)
        AddMessage(msg);
    }

    /// <summary>
    /// Validates triggers for persistence
    /// </summary>
    private void ValidatePersistence()
    {
      List<StratusTriggerBase> persistents = new List<StratusTriggerBase>();
      persistents.AddRange(triggers.FindAll(x => x.persistent));
      if (persistents.NotEmpty())
      {
        string msg = $"Triggers marked as persistent ({persistents.Count}):";
        foreach (var t in persistents)
          msg += $"\n- {t.GetType().Name} : <i>{t.description}</i>";
        AddMessage(new ObjectValidation(msg, ObjectValidation.Level.Warning, target));
      }
      else
        AddMessage($"There are no persistent triggers in this system", UnityEditor.MessageType.Info, null);
    }
    
    private void ValidateConnections()
    {      
      List<StratusTriggerBase> disconnected = new List<StratusTriggerBase>();
      foreach (var t in triggers)
      {
        if (!IsConnected(t))
          disconnected.Add(t);
      }
      foreach(var t in triggerables)
      {
        if (!IsConnected(t))
          disconnected.Add(t);
      }

      if (disconnected.NotEmpty())
      {
        string msg = $"Triggers marked as disconnected ({disconnected.Count}):";
        foreach (var t in disconnected)
          msg += $"\n- {t.GetType().Name} : <i>{t.description}</i>";
        AddMessage(msg, UnityEditor.MessageType.Warning, null);
      }
    }

    private void ValidateNull()
    {
      foreach (var t in triggers)
      {
        var validation = ObjectValidation.NullReference(t, $"<i>{t.description}</i>");
        if (validation != null) AddMessage(validation);
      }
      foreach (var t in triggerables)
      {
        var validation = ObjectValidation.NullReference(t, $"<i>{t.description}</i>");
        if (validation != null) AddMessage(validation);
      }
    }
    
    /// <summary>
    /// Runs a specific validation function (after some pre-steps)
    /// </summary>
    /// <param name="validateFunc"></param>
    private void Validate(System.Action validateFunc)
    {
      this.messages.Clear();
      validateFunc();
    }

    private StratusTriggerSystem.ConnectionStatus GetStatus(Trigger trigger)
    {
      StratusTriggerSystem.ConnectionStatus status = StratusTriggerSystem.ConnectionStatus.Disconnected;
      if (selected)
      {
        if (selected == trigger)
          status = StratusTriggerSystem.ConnectionStatus.Selected;
        else if (selectedTriggerable && connectedTriggers.ContainsKey(trigger) && connectedTriggers[trigger])
          status = StratusTriggerSystem.ConnectionStatus.Connected;
      }

      if (!IsConnected(trigger) && selected != trigger)
        status = StratusTriggerSystem.ConnectionStatus.Disjoint;
      return status;
    }

    private StratusTriggerSystem.ConnectionStatus GetStatus(StratusTriggerable triggerable)
    {
      StratusTriggerSystem.ConnectionStatus status = StratusTriggerSystem.ConnectionStatus.Disconnected;
      if (selected)
      {
        if (selected == triggerable)
          status = StratusTriggerSystem.ConnectionStatus.Selected;
        else if (selectedTrigger && connectedTriggerables.ContainsKey(triggerable) && connectedTriggerables[triggerable])
          status = StratusTriggerSystem.ConnectionStatus.Connected;
      }
      if (!IsConnected(triggerable) && selected != triggerable)
        status = StratusTriggerSystem.ConnectionStatus.Disjoint;
      return status;
    }

  }
}