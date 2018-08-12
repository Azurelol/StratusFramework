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
      AddConstraint(() => target.initialStateConfiguration == Stateful.InitialStateConfiguration.OnCallbackFinished, nameof(Stateful.onInitialState));
      AddConstraint(() => target.initialStateConfiguration == Stateful.InitialStateConfiguration.OnDelay, nameof(Stateful.delay));
    }
  }

}