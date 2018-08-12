/******************************************************************************/
/*!
@file   CombatActionItem.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /// <summary>
  /// 
  /// </summary>
  public class CombatActionItem : CombatAction
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public Inventory.ConsumableEntry Item;
    /// <summary>
    /// All items have the same cast time?
    /// </summary>
    //static float DefaultCastTime = 0.1f;
    /// <summary>
    /// Items all have the same throw range?
    /// </summary>
    static float DefaultRange = 10.0f;
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// CombatActionItem constructor that takes a reference to an item belonging in the inventory.
    /// </summary>
    public CombatActionItem(Inventory.ConsumableEntry item, CombatController user, CombatController target)
      : base(user, target, DefaultRange, new Timings())
    {
      this.Item = item;      
    }

    public override string description
    {
      get
      {
        return this.Item.Item.Description;
      }
    }

    public override string name
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Executes the specified skill on the given target. This is called
    /// after the action has finished its cast time.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    protected override void OnExecute(CombatController user, CombatController target)
    {
      if (Item != null)
        this.Item.Use(user, target);
    }

    protected override void OnTrigger(CombatController user, CombatController target)
    {
      // Items always succeed on their execution
      //var seq = Actions.Sequence(user);
      //Actions.Delay(seq, this.AnimationDuration);
      //Actions.Call(seq, SignalActionIsReady);
    }

    protected override void OnStart(CombatController user, CombatController target)
    {
      
    }

    protected override void OnCasting(CombatController user, CombatController target, float step)
    {
      
    }

    protected override void OnEnd()
    {
      throw new NotImplementedException();
    }

    protected override void OnCancel()
    {
      throw new NotImplementedException();
    }
  }

}