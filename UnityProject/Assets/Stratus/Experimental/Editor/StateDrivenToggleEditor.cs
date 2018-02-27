using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(StateDrivenToggle), true)]
  public class StateDrivenToggleEditor : BaseEditor<StateDrivenToggle>
  {
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