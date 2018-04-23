/******************************************************************************/
/*!
@file   LinkEditor.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
//using UnityEngine;
//using UnityEditor;
//using Stratus;

/**************************************************************************/
/*!
@class LinkEditor 
*/
/**************************************************************************/
namespace LinkInterfaceSystem
{
  //[CustomPropertyDrawer(typeof(Link.LinkNavigation))]
  //public class LinkNavigationDrawer : PropertyDrawer
  //{
  //  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  //  {
  //    // Using BeginProperty / EndProperty on the parent property means that
  //    // prefab override logic works on the entire property.
  //    EditorGUI.BeginProperty(position, label, property);

  //    // Draw label
  //    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

  //    // Draw the navigation property
  //    var propertyNavigation = property.FindPropertyRelative("Navigation");
  //    EditorGUILayout.PropertyField(propertyNavigation, new GUIContent("Navigation"));

  //    // Draw fields
  //    var mode = (Link.LinkNavigation.NavigationMode)propertyNavigation.enumValueIndex;
  //    switch (mode)
  //    {
  //      case Link.LinkNavigation.NavigationMode.Automatic:
  //        break;
  //      case Link.LinkNavigation.NavigationMode.Manual:
  //        var propertyOnNavigateUp = property.FindPropertyRelative("OnNavigateUp");
  //        var propertyOnNavigateDown = property.FindPropertyRelative("OnNavigateDown");
  //        var propertyOnNavigateLeft = property.FindPropertyRelative("OnNavigateLeft");
  //        var propertyOnNavigateRight = property.FindPropertyRelative("OnNavigateRight");
  //        EditorGUILayout.PropertyField(propertyOnNavigateUp, new GUIContent("OnNavigateUp"));
  //        EditorGUILayout.PropertyField(propertyOnNavigateDown, new GUIContent("OnNavigateDown"));
  //        EditorGUILayout.PropertyField(propertyOnNavigateLeft, new GUIContent("OnNavigateLeft"));
  //        EditorGUILayout.PropertyField(propertyOnNavigateRight, new GUIContent("OnNavigateRight"));
  //        break;
  //    }

  //    //EditorGUI.indentLevel = indent;

  //    EditorGUI.EndProperty();
  //    //if (property)

  //  }
  //}

}

//[CustomEditor(typeof(Link), true)]
//public class LinkEditor : Editor
//{
//  public SerializedProperty
//    PropertyNavigation,
//    PropertyOnNavigateUp,
//    PropertyOnNavigateDown,
//    PropertyOnNavigateLeft,
//    PropertyOnNavigateRight;

//  void OnEnable()
//  {
//    // Set up the serialized properties
//    PropertyNavigation = serializedObject.FindProperty("Navigation");
//    PropertyOnNavigateUp = serializedObject.FindProperty("OnNavigateUp");
//    PropertyOnNavigateDown = serializedObject.FindProperty("OnNavigateDown");
//    PropertyOnNavigateLeft = serializedObject.FindProperty("OnNavigateLeft");
//    PropertyOnNavigateRight = serializedObject.FindProperty("OnNavigateRight");
//  }

//  public override void OnInspectorGUI()
//  {
//    //var link = target as Link;
//    //if (link.Navigation == Link.NavigationMode.Automatic)
//    //{
//    //
//    //}
//    //else
//    //{
//    //  EditorGUILayout.PropertyField(PropertyOnNavigateUp, new GUIContent("OnNavigateUp"));
//    //  EditorGUILayout.PropertyField(PropertyOnNavigateDown, new GUIContent("OnNavigateDown"));
//    //  EditorGUILayout.PropertyField(PropertyOnNavigateLeft, new GUIContent("OnNavigateLeft"));
//    //  EditorGUILayout.PropertyField(PropertyOnNavigateRight, new GUIContent("OnNavigateRight"));
//    //}


//    serializedObject.Update();
//    EditorGUILayout.PropertyField(PropertyNavigation);
//    Link.NavigationMode mode = (Link.NavigationMode)PropertyNavigation.enumValueIndex;
//    switch (mode)
//    {
//      case Link.NavigationMode.Automatic:
//        break;
//      case Link.NavigationMode.Manual:
//        EditorGUILayout.PropertyField(PropertyOnNavigateUp, new GUIContent("OnNavigateUp"));
//        EditorGUILayout.PropertyField(PropertyOnNavigateDown, new GUIContent("OnNavigateDown"));
//        EditorGUILayout.PropertyField(PropertyOnNavigateLeft, new GUIContent("OnNavigateLeft"));
//        EditorGUILayout.PropertyField(PropertyOnNavigateRight, new GUIContent("OnNavigateRight"));
//        break;
//    }

//    serializedObject.ApplyModifiedProperties();
//  }
//}

//[CustomEditor(typeof(WindowLink), true)]
//public class WindowLinkEditor : Editor
//{
//  public override void OnInspectorGUI()
//  {
//    base.OnInspectorGUI();

//    // Additional 
//  }
//}



//}