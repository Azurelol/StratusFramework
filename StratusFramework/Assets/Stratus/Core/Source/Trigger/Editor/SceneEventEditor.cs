using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(SceneEvent))]
  public class SceneEventEditor : TriggerableEditor<SceneEvent>
  {
    SceneEvent sceneEvent => target as SceneEvent;

    protected override void OnTriggerableEditorEnable()
    {

    }

    protected override bool DrawDeclaredProperties()
    {
      bool changed = false;
      changed |= DrawSerializedProperty(declaredProperties.Item2[0], serializedObject);
      if (sceneEvent.type == SceneEvent.Type.Load || sceneEvent.type == SceneEvent.Type.Unload)
      {
        changed |= DrawSerializedProperty(declaredProperties.Item2[1], serializedObject);
      }
      if (sceneEvent.type == SceneEvent.Type.Load)
      {
        changed |= DrawSerializedProperty(declaredProperties.Item2[2], serializedObject);
      }
      return changed;
    }


  }
}