using UnityEngine;
using Stratus;
using UnityEditor;
using Rotorz.ReorderableList;
using Stratus.Dependencies.TypeReferences;

namespace Stratus
{
  namespace AI
  {
    [CustomEditor(typeof(StatefulAction))]
    public class StatefulActionEditor: ScriptableEditor<StatefulAction>
    {
      private UnityEditor.Editor ActionEditor;
      SerializedProperty Type;
      //SerializedProperty Preconditions;
      //SerializedProperty Effects;
      //SerializedProperty Cost;

      protected override void OnStratusEditorEnable()
      {
        Type = this.GetSerializedProperty(nameof(StatefulAction.type));
        this.AddArea(ModifyAction);
      }      


      void ModifyAction(Rect rect)
      {
        EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
        // Set the action
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(Type);

        if (GUILayout.Button("Set", EditorStyles.miniButtonRight))
        {
          target.action = (Action)StratusEditorUtility.Instantiate(target.type.Type);
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
