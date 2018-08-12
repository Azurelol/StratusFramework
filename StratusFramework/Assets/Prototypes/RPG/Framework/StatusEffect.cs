/******************************************************************************/
/*!
@file   StatusEffect.cs
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
  /// Applies a status to the target.
  /// </summary>
  public class StatusEffect : EffectAttribute
  {
    public Status Status;

    public override void OnInspect()
    {
      #if UNITY_EDITOR
      Status = UnityEditor.EditorGUILayout.ObjectField("Status", Status, typeof(Status), true) as Status;
      #endif
      //Status = EditorHelper.Object<Status>("Status", Status);
    }

    protected override void OnApply(CombatController caster, CombatController target)
    {
      // Apply the status to the target. This will get it added to the targets's
      // list of active statuses for its duration
      Status.Apply(caster, target);
    }
  }

}