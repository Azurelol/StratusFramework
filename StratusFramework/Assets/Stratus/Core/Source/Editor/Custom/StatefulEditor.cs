using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(Stateful))]
  public class StatefulEditor : BehaviourEditor<Stateful>
  {
    protected override void OnStratusEditorEnable()
    {
      AddConstraint(nameof(Stateful.onInitialState), () => target.initialStateConfiguration == Stateful.InitialStateConfiguration.OnCallbackFinished);
      AddConstraint(nameof(Stateful.delay), () => target.initialStateConfiguration == Stateful.InitialStateConfiguration.OnDelay);
    }
  }

}