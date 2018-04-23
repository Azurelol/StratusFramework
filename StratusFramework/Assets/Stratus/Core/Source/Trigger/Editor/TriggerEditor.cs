using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;
using System.Reflection;
using System;

namespace Stratus
{
  [CustomEditor(typeof(Trigger), true), CanEditMultipleObjects]
  public class TriggerEditor : TriggerBaseEditor<Trigger>
  {
    internal override void OnTriggerBaseEditorEnable()
    {
    }
  }

  [CustomEditor(typeof(Triggerable), true), CanEditMultipleObjects]
  public class TriggerableEditor : TriggerBaseEditor<Triggerable>
  {
    internal override void OnTriggerBaseEditorEnable()
    {
    }
  }

  [CustomEditor(typeof(Trigger), true), CanEditMultipleObjects]
  public abstract class TriggerEditor<T> : TriggerEditor where T : Trigger
  {
    /// <summary>
    /// The target cast as the declared trigger type
    /// </summary>
    protected T trigger { get; private set; }

    protected abstract void OnTriggerEditorEnable();

    internal override void OnTriggerBaseEditorEnable()
    {
      trigger = base.target as T;
      OnTriggerEditorEnable();
    }
  }

  [CustomEditor(typeof(Triggerable), true), CanEditMultipleObjects]
  public abstract class TriggerableEditor<T> : TriggerableEditor where T : Triggerable
  {
    /// <summary>
    /// The target cast as the declared triggerable type
    /// </summary>
    protected T triggerable { get; private set; }

    protected abstract void OnTriggerableEditorEnable();

    internal override void OnTriggerBaseEditorEnable()
    {
      triggerable = base.target as T;
      OnTriggerableEditorEnable();
    }
  }



}