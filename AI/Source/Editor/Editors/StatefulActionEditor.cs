using UnityEditor;
using UnityEngine;

namespace Stratus
{
	namespace AI
	{
		[CustomEditor(typeof(StatefulAction))]
		public class StatefulActionEditor : StratusScriptableEditor<StatefulAction>
		{
			private UnityEditor.Editor ActionEditor;
			private SerializedProperty Type;
			//SerializedProperty Preconditions;
			//SerializedProperty Effects;
			//SerializedProperty Cost;

			protected override void OnStratusEditorEnable()
			{
				this.Type = this.GetSerializedProperty(nameof(StatefulAction.type));
				this.AddArea(this.ModifyAction);
			}

			private void ModifyAction(Rect rect)
			{
				EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
				// Set the action
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(this.Type);

				if (GUILayout.Button("Set", EditorStyles.miniButtonRight))
				{
					this.target.task = (StratusAITask)StratusEditorUtility.Instantiate(this.target.type.Type);
				}
				EditorGUILayout.EndHorizontal();

				// If an action has been set, show it
				//if (target.action != null)
				//  ActionEditor = UnityEditor.Editor.CreateEditor(StatefulAction.action);
				//ActionEditor.DrawDefaultInspector();

			}


		}
	}
}
