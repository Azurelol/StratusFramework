using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

namespace Stratus
{
  public abstract class FilteredPropertyDrawer : StratusPropertyDrawer
  {
    //private static StringBuilder propertyNames = new StringBuilder();
    //private static PropertyGroup propertyGroup = new PropertyGroup();
    private float height = 0f;
    //private int propertyCount = 0;

    //public struct PropertyGroup
    //{      
    //
    //  public void Add(string propertyName)
    //  {
    //    propertyNames.AppendLine(propertyName);
    //  }
    //  
    //
    //}

    protected override float GetPropertyHeight(SerializedProperty property)
    {
      return height;
    }

    protected override void OnDrawProperty(Rect position, SerializedProperty property)
    {
      //propertyCount = 0;
      //propertyNames.Clear();

      SerializedProperty[] properties = GetProperties(property);//GetProperties(propertyGroup);
      //height = properties.Length;
      height = 0;
      for(int i = 0; i < properties.Length; ++i)
      {
        //SerializedProperty childProperty = property.FindPropertyRelative(propertyNames.)
        SerializedProperty childProperty = properties[i];
        float childPropertyHeight = EditorGUI.GetPropertyHeight(childProperty);
        position.height = childPropertyHeight;
        EditorGUI.PropertyField(position, childProperty);
        position.y += childPropertyHeight;
        height += childPropertyHeight;
      }

    }

    //protected abstract void GetProperties(PropertyGroup propertyGroup);
    protected abstract SerializedProperty[] GetProperties(SerializedProperty property);
  }

}