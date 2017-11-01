/******************************************************************************/
/*!
@file   ReadOnlyDrawer.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@note  Resource: It3ration
       http://answers.unity3d.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using Stratus;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
  public class ReadOnlyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      GUI.enabled = false;
      EditorGUI.PropertyField(position, property, label);
      GUI.enabled = true;
    }
  }

}
#endif