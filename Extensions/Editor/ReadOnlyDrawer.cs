/******************************************************************************/
/*!
@file   ReadOnlyDrawer.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@note  Resource: It3ration
       http://answers.unity3d.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEditor;

/**************************************************************************/
/*!
@class ReadOnlyAttribute 
*/
/**************************************************************************/
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
  //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  //{
  //  return EditorGUI.GetPropertyHeight(property, label, true);
  //}

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    GUI.enabled = false;
    EditorGUI.PropertyField(position, property, label);
    GUI.enabled = true;
  }



}
