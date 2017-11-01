using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  /// <summary>
  /// Reads into Unity's input manager to provide additional information
  /// </summary>
  [InitializeOnLoad]
  public class InputManagerUtility
  {
    public static string[] axes { get; private set; }

    static InputManagerUtility()
    {
      ReadAxes();
    }

    private static void ReadAxes()
    {
      var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
      var obj = new SerializedObject(inputManager);
      var axisArray = obj.FindProperty("m_Axes");
      var axisList = new List<string>();
      for(var i = 0; i < axisArray.arraySize; ++i)
      {
        var axis = axisArray.GetArrayElementAtIndex(i);
        var name = axis.FindPropertyRelative("m_Name").stringValue;
        axisList.Add(name);
      }

      InputManagerUtility.axes = axisList.ToArray();
    }

  }
}