using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(DocumentationAsset))]
  public class DocumentationAssetEditor : StratusScriptableEditor<DocumentationAsset>
  {
    protected override void OnStratusEditorEnable()
    {
      
    }

    //public override void OnBaseEditorGUI()
    //{
    //  //DrawReorderableList(target.elements, DrawElement);
    //
    //  foreach (var element in target.elements)
    //    Draw(element);      
    //}

    private void Draw(DocumentationAsset.Element element)
    {
      if (element == null)
        return;

      switch (element.type)
      {
        case DocumentationAsset.ElementType.Text:
          break;

        case DocumentationAsset.ElementType.Image:
          break;

        case DocumentationAsset.ElementType.Video:
          break;
      }
    }

    private DocumentationAsset.Element DrawElement(Rect position, DocumentationAsset.Element element)
    {
      //element.type = (DocumentationAsset.ElementType)EditorGUI.(position, element.type);
      switch (element.type)
      {
        case DocumentationAsset.ElementType.Text:
          EditorGUILayout.TextArea(element.text);
          break;

        case DocumentationAsset.ElementType.Image:
          break;

        case DocumentationAsset.ElementType.Video:
          break;
      }
      return element;
    }


  }

  [CustomPropertyDrawer(typeof(DocumentationAsset.Element))]
  public class DocumentationAssetElementDrawer : FilteredPropertyDrawer
  {
    protected override SerializedProperty[] GetProperties(SerializedProperty property)
    {
      SerializedProperty typeProperty = property.FindPropertyRelative(nameof(DocumentationAsset.Element.type));
      DocumentationAsset.ElementType type = GetEnumValue<DocumentationAsset.ElementType>(property, nameof(DocumentationAsset.Element.type));
      string secondPropertyName = string.Empty;
      switch (type)
      {
        case DocumentationAsset.ElementType.Text:
          secondPropertyName = nameof(DocumentationAsset.Element.text);
          break;
        case DocumentationAsset.ElementType.Image:
          secondPropertyName = nameof(DocumentationAsset.Element.image);
          break;
        case DocumentationAsset.ElementType.Video:
          secondPropertyName = nameof(DocumentationAsset.Element.video);
          break;
      }
      SerializedProperty secondProperty = property.FindPropertyRelative(secondPropertyName);
      return new SerializedProperty[] { typeProperty, secondProperty };
    }
  }

  //[CustomPropertyDrawer(typeof(DocumentationAsset.Element))]
  //public class DocumentationAssetElementDrawer : DualDynamicPropertyDrawer
  //{
  //  //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  //  //{
  //  //  return GetPropertyHeight(GetSecondProperty(property), label);
  //  //}
  //
  //  protected override SerializedProperty GetFirstProperty(SerializedProperty property)
  //  {
  //    return property.FindPropertyRelative(nameof(DocumentationAsset.Element.type));
  //  }
  //
  //  protected override SerializedProperty GetSecondProperty(SerializedProperty property)
  //  {
  //    DocumentationAsset.ElementType type = GetEnumValue<DocumentationAsset.ElementType>(property, nameof(DocumentationAsset.Element.type));
  //    string secondPropertyName = string.Empty;
  //    switch (type)
  //    {
  //      case DocumentationAsset.ElementType.Text:
  //        secondPropertyName = nameof(DocumentationAsset.Element.text);
  //        break;
  //      case DocumentationAsset.ElementType.Image:
  //        secondPropertyName = nameof(DocumentationAsset.Element.image);
  //        break;
  //      case DocumentationAsset.ElementType.Video:
  //        secondPropertyName = nameof(DocumentationAsset.Element.video);
  //        break;
  //    }
  //    return property.FindPropertyRelative(secondPropertyName);
  //  }
  //}


}