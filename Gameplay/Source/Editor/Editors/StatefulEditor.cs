using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(StratusStatefulObject))]
  public class StatefulEditor : StratusBehaviourEditor<StratusStatefulObject>
  {
    protected override void OnStratusEditorEnable()
    {
      AddConstraint(() => target.initialStateConfiguration == StratusStatefulObject.InitialStateConfiguration.OnCallbackFinished, nameof(StratusStatefulObject.onInitialState));
      AddConstraint(() => target.initialStateConfiguration == StratusStatefulObject.InitialStateConfiguration.OnDelay, nameof(StratusStatefulObject.delay));
    }
  }

}