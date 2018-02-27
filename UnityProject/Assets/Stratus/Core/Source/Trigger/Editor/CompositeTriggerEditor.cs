using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Stratus
{
  [CustomEditor(typeof(CompositeTrigger))]
  public class CompositeTriggerEditor : BaseEditor<CompositeTrigger>
  {
    //CompositeTrigger composite => target as CompositeTrigger;
    //
    //private SerializedProperty typeProp => declaredProperties.Item2[0];
    //private SerializedProperty criteriaProp => declaredProperties.Item2[1];
    //private SerializedProperty neededProp => declaredProperties.Item2[2];
    //private SerializedProperty triggersProp => declaredProperties.Item2[3];
    //private SerializedProperty triggerablesProp => declaredProperties.Item2[4];

    protected override void OnBaseEditorEnable()
    {
      propertyConstraints.Add(propertyMap["triggers"], () => target.type == CompositeTrigger.Type.Trigger);
      propertyConstraints.Add(propertyMap["triggerables"], () => target.type == CompositeTrigger.Type.Triggerable);
      //propertyGroupDrawOverrides.Add(typeof(CompositeTrigger), DrawCompositeTriggerProperties);

    } 
    
    //protected override void DrawDeclaredProperties()
    //{
    //  DrawSerializedProperty(typeProp, serializedObject);
    //  DrawSerializedProperty(criteriaProp, serializedObject);
    //
    //  if (composite.criteria == CompositeTrigger.Criteria.Subset)
    //    composite.needed = EditorGUILayout.IntSlider(composite.needed, 1, composite.count);
    //  //  //DrawSerializedProperty(neededProp, serializedObject);
    //  
    //  if (composite.type == CompositeTrigger.Type.Trigger)
    //  {
    //    DrawSerializedProperty(triggersProp, serializedObject);      
    //  }
    //  else if (composite.type == CompositeTrigger.Type.Triggerable)
    //  {
    //
    //    DrawSerializedProperty(triggerablesProp, serializedObject);      
    //  }
    //
    //}

  }

}