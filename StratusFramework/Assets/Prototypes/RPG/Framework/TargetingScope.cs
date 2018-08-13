/******************************************************************************/
/*!
@file   TargetingScope.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;
using System.Linq;

namespace Prototype
{
  /// <summary>
  /// Handles targetting for skills
  /// </summary>
  [Serializable]
  public class TargetingScope
  {
    public enum Type { Single, Radius, Line, Group }
    public Type Scope = Type.Single;
    [Range(1.0f, 10.0f)] public float Length;
    [Range(1.0f, 10.0f)] public float Width;    
    float Radius { get { return Length; } }
        
    /// <summary>
    /// Draws this targeting scope.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    public void Draw(CombatController caster, CombatController target)
    {
      switch (this.Scope)
      {
        case Type.Single:
          break;
        case Type.Radius:
          break;
        case Type.Line:
          break;
        case Type.Group:
          break;
      }
    }

    /// <summary>
    /// Finds targets from the specified type that fit the parameters of the
    /// selected scope.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public CombatController[] FindTargets(CombatController caster, CombatController target, Combat.TargetingParameters type)
    {
      //Trace.Script("Primary target = " + target.Name + " with scope = " + Scope, caster);
      var targets = new CombatController[0];
      switch (this.Scope)
      {
        case Type.Single:
          targets = new CombatController[] { target };
          break;
        case Type.Radius:
          throw new NotImplementedException();
          //targets = caster.FindTargetsOfType(type, this.Radius);
          //break;
        case Type.Group:
          throw new NotImplementedException();
          //targets = caster.FindTargetsOfType(type);
          //break;
      }

      return targets;
    }

    //public static CombatController[] FindAdditionalTargets(CombatController target)
    //{
    //
    //}


    /// <summary>
    /// Given an array of combatants, filters out those who aren't valid targets for the given targeting parameters.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="availableTargets"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static CombatController[] FilterTargets(CombatController caster, CombatController[] availableTargets, Combat.TargetingParameters type)
    {
      CombatController[] targets = null;

      // SELF
      if (type == Combat.TargetingParameters.Self)
      {
        targets = new CombatController[1] { caster }; //.Add(caster);
      }
      // ALLIES
      else if (type == Combat.TargetingParameters.Ally)
      {
        switch (caster.faction)
        {
          case CombatController.Faction.Player:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Player select target).ToArray();
            break;

          case CombatController.Faction.Hostile:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Hostile select target).ToArray();
            break;
          case CombatController.Faction.Neutral:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Neutral select target).ToArray();
            break;
        }
      }
      // ENEMIES
      else if (type == Combat.TargetingParameters.Enemy)
      {
        switch (caster.faction)
        {
          case CombatController.Faction.Player:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Hostile select target).ToArray();
            break;

          case CombatController.Faction.Hostile:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Player select target).ToArray();
            break;
          case CombatController.Faction.Neutral:
            targets = (from CombatController target in availableTargets
                       where (target.faction == CombatController.Faction.Player || target.faction == CombatController.Faction.Hostile) 
                       select target).ToArray();
            break;

        }
      }
      else if (type == Combat.TargetingParameters.Any)
      {
        targets = availableTargets;
      }

      return (from CombatController controller in targets where controller.currentState == CombatController.State.Active select controller).ToArray();
    }

    

    //public abstract void OnInspect();
    //public abstract void OnDraw();
    //public abstract CombatController[] OnTarget(CombatController caster, CombatController target);
  }

  ///**************************************************************************/
  ///*!
  //@class TargetingScopeSingle 
  //*/
  ///**************************************************************************/
  //public class TargetingScopeSingle : TargetingScope
  //{
  //  public override void OnDraw()
  //  {
  //    // Draw a small circle around the target
  //  }

  //  public override void OnInspect()
  //  {      
  //    // No additional parameters are needed
  //  }

  //  public override CombatController[] OnTarget(CombatController caster, CombatController target)
  //  {
  //    // Just return the target itself
  //    return new CombatController[] { target };
  //  }
  //}

  ///**************************************************************************/
  ///*!
  //@class TargetingScopeRadius 
  //*/
  ///**************************************************************************/
  //public class TargetingScopeRadius : TargetingScope
  //{
  //  [Range(1.0f, 10.0f)] float Radius;

  //  public override void OnInspect()
  //  {
  //    this.Radius = EditorHelper.Field("Radius", this.Radius);
  //  }

  //  public override void OnDraw()
  //  {
  //    // Draw a big circle on the target of the specified radius
  //  }

  //  public override CombatController[] OnTarget(CombatController caster, CombatController target)
  //  {
  //    // Find others within the radius of the given target (of the same party)
  //    return target.FindTargetsOfType(CombatController.TargetType.Ally, this.Radius);
  //  }
  //}

  ///**************************************************************************/
  ///*!
  //@class TargetingScopeLine 
  //*/
  ///**************************************************************************/
  //public class TargetingScopeLine : TargetingScope
  //{
  //  [Range(1.0f, 10.0f)] float Length;
  //  [Range(1.0f, 10.0f)] float Width;

  //  public override void OnInspect()
  //  {
  //    this.Length = EditorHelper.Field("Length", this.Length);
  //    this.Width = EditorHelper.Field("Width", this.Width);
  //  }

  //  public override void OnDraw()
  //  {
  //    // Start drawing a thick line of specified length and width
  //    // from the caster to the target
  //  }

  //  public override CombatController[] OnTarget(CombatController caster, CombatController target)
  //  {
  //    // Find any targets within the line
  //    throw new NotImplementedException();
  //  }
  //}

  ///**************************************************************************/
  ///*!
  //@class TargetingScopeGroup 
  //*/
  ///**************************************************************************/
  //public class TargetingScopeGroup : TargetingScope
  //{
  //  public override void OnInspect()
  //  {
  //  }

  //  public override void OnDraw()
  //  {
  //    // Draw targetting circles for each member of the group
  //  }

  //  public override CombatController[] OnTarget(CombatController caster, CombatController target)
  //  {
  //    // Return all allies of the target
  //    return target.FindTargetsOfType(CombatController.TargetType.Ally);
  //  }
  //}

}