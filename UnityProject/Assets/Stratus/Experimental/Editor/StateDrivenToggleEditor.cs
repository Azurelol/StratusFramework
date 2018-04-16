using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(StateDrivenToggle), true)]
  public class StateDrivenToggleEditor : BehaviourEditor<StateDrivenToggle>
  {
    protected override void OnBaseEditorEnable()
    {
      throw new System.NotImplementedException();
    }

    //protected override void Configure()
    //{
    //  SerializedProperty extent = propertyMap["extent"];
    //  SerializedProperty activeStates = propertyMap["activeStates"];
    //  propertyConstraints.Add(activeStates, () => 
    //  {
    //    if (extent.en)
    //    return false;
    //  });
    //}

  }

}