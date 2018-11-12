/******************************************************************************/
/*!
@file   TagPropertyDrawer.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@note   Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
        Altered by Brecht Lecluyse http://www.brechtos.com
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(TagAttribute))]
  public class TagPropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (property.propertyType == SerializedPropertyType.String)
      {
        EditorGUI.BeginProperty(position, label, property);

        var attrib = this.attribute as TagAttribute;
        if (attrib.UseDefaultTagFieldDrawer)
        {
          property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }
        else
        {
          // Generate the taglist + custom tags
          var tagList = new List<string>();
          tagList.Add("<NoTag>");
          tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
          var propertyString = property.stringValue;

          int index = -1;
          // If the tag is empty, the first index will be the special <NoTag> entry
          if (propertyString == "")
          {
            index = 0;
          }
          // Check if there is an entry that matches the entry and get the index
          // Skip index 0, of course
          else
          {
            for (var i = 1; i < tagList.Count; i++)
            {
              if (tagList[i] == propertyString)
              {
                index = i;
                break;
              }
            }
          }

          // Draw the popup box with the current selected index
          index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

          // Adjust the actual string value of the property based on the selection
          if (index >= 1)
            property.stringValue = tagList[index];
          else
            property.stringValue = "";

          EditorGUI.EndProperty();
        }
      }
      else
      {
        EditorGUI.PropertyField(position, property, label);
      }

    }
  }
}

#endif