using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(ExposeProperty))]
  public class ExposePropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      base.OnGUI(position, property, label);
    }
  }


  [InitializeOnLoad]
  public static class ExposePropertyManager
  {
    static ExposePropertyManager()
    {
    }

    /// <summary>
    /// Updates all property drawers
    /// </summary>
    static void Update()
    {
      if (Selection.activeGameObject == null)
      {
        Trace.Script("Inspectinig nothing");
        return;
      }

      

      var components = Selection.activeGameObject.GetComponents<MonoBehaviour>();
      //var exposedProperties = new List<ExposeProperty>();
      int count = 0;

      foreach(var component in components)
      {
        var properties = component.GetType().GetProperties();
        foreach(var property in properties)
        {
          var exposedProperties = property.GetCustomAttributes(typeof(ExposeProperty), true);
          bool isExposed = exposedProperties.Length != 0;
          if (isExposed)
          {
            //var exposedProperty = exposedProperties[0] as ExposeProperty;
            count++;
          }

        }
      }

      Trace.Script("Inspecting " + count + " properties on " + Selection.activeGameObject.name);



    }


  }

}
