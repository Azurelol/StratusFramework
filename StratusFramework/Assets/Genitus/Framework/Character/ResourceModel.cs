using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;

namespace Genitus.Models
{
  /// <summary>
  /// Skills consume a finite amount of mana
  /// </summary>
  public class ManaComponent : Character.ResourceModel.Component 
  {
    public int mana;

    public override void OnUpdate(float step)
    {      
    }

    public override void OnUsage()
    {      
    }
  }

  [Serializable]
  public class ManaModel : Character.ResourceModel
  {
    public int mana;

    public override void Use()
    {      
    }
  }


  /// <summary>
  /// Skills, once activated, cannot be used until a cooldown period has passed
  /// </summary>
  public class CooldownComponent : Character.ResourceModel.Component
  {
    /// <summary>
    /// Time required before a skill can be used again after activation
    /// </summary>
    [Tooltip("Time required before the skill can be used again after activation")]
    public VariableAttribute cooldown;

    private Cooldown timer;

    public override void OnUsage()
    {
      timer.Activate();
    }

    public override void OnUpdate(float timeStep)
    {
      timer.Update(timeStep);
    }
  }

}