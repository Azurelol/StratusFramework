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
    public class StatefulActionEditor: Editor
    {
      StatefulAction StatefulAction;

      // Properties
      //SerializedProperty BlackboardAsset;
      SerializedProperty Type;

      private Editor ActionEditor;
      SerializedProperty Preconditions;
      SerializedProperty Effects;
      SerializedProperty Cost;


      private void OnEnable()
      {
        StatefulAction = target as StatefulAction;

        //BlackboardAsset = serializedObject.FindProperty("BlackboardAsset");
        Type = serializedObject.FindProperty("Type");        

        Preconditions = serializedObject.FindProperty("Preconditions");
        Effects = serializedObject.FindProperty("Effects");
        Cost = serializedObject.FindProperty("Cost");
      }

      public override void OnInspectorGUI()
      {
        //EditorGUILayout.LabelField("Assets", EditorStyles.boldLabel);
        //EditorGUILayout.PropertyField(BlackboardAsset);        
        ModifyAction();
        EditorGUILayout.LabelField("Planner", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(Cost);
        EditorGUILayout.PropertyField(Preconditions);
        EditorGUILayout.PropertyField(Effects);
      }

      void ModifyAction()
      {
        EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
        // Set the action
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(Type);
        if (GUILayout.Button("Set", EditorStyles.miniButtonRight))
        {
          var action = StatefulAction.AddInstanceToAsset(StatefulAction.Type.Type.DeclaringType) as Action;
          StatefulAction.Action = action;
        }
        EditorGUILayout.EndHorizontal();

        // If an action has been set, show it
        if (StatefulAction.Action && !ActionEditor)
          ActionEditor = Editor.CreateEditor(StatefulAction.Action);
        ActionEditor.DrawDefaultInspector();

      }
      


    }
  }
}
