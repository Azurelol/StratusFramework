using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
	[CustomEditor(typeof(StratusSceneEvent))]
	public class SceneEventEditor : TriggerableEditor<StratusSceneEvent>
	{
		StratusSceneEvent sceneEvent => target as StratusSceneEvent;

		protected override void OnTriggerableEditorEnable()
		{

		}

		protected override bool DrawDeclaredProperties()
		{
			bool changed = false;
			changed |= DrawSerializedProperty(declaredProperties.Item2[0].unitySerialized, serializedObject);
			if (sceneEvent.type == StratusSceneEvent.Type.Load || sceneEvent.type == StratusSceneEvent.Type.Unload)
			{
				changed |= DrawSerializedProperty(declaredProperties.Item2[1].unitySerialized, serializedObject);
			}
			if (sceneEvent.type == StratusSceneEvent.Type.Load)
			{
				changed |= DrawSerializedProperty(declaredProperties.Item2[2].unitySerialized, serializedObject);
			}
			return changed;
		}


	}
}