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
  public class TriggerEditor : BaseEditor<Trigger>
  {
  }
  
  [CustomEditor(typeof(Triggerable), true), CanEditMultipleObjects]
  public class TriggerableEditor : BaseEditor<Triggerable>
  {
  }

  [CustomEditor(typeof(SceneEvent))]
  public class SceneEventEditor : TriggerableEditor
  {
    SceneEvent sceneEvent => target as SceneEvent;

    protected override void DrawDeclaredProperties()
    {
      DrawSerializedProperty(declaredProperties.Item2[0], serializedObject);
      if (sceneEvent.type == SceneEvent.Type.Load || sceneEvent.type == SceneEvent.Type.Unload)
      {
        DrawSerializedProperty(declaredProperties.Item2[1], serializedObject);
      }
      if (sceneEvent.type == SceneEvent.Type.Load)
      {
        DrawSerializedProperty(declaredProperties.Item2[2], serializedObject);
      }

    }

  }

}