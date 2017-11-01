using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace AI
  {
    [CustomEditor(typeof(Agent))]
    public class AgentEditor : Editor
    {
      //Agent Agent;
      //
      //private void OnEnable()
      //{
      //  Agent = target as Agent;
      //}
      //
      //public override void OnInspectorGUI()
      //{
      //  EditorGUILayout.PropertyField(serializedObject.FindProperty("Active"));
      //  EditorGUILayout.PropertyField(serializedObject.FindProperty("BlackBoardPrototype"), new GUIContent("Blackboard"));
      //
      //  //base.OnInspectorGUI();
      //}
    }  

  }
}
