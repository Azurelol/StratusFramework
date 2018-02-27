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
    public static string[] axesNames { get; private set; }
    public static Dictionary<string, int> axesIndex { get; private set; }
    public static SerializedProperty axesArray { get; private set; } 

    static InputManagerUtility()
    {
      ReadAxes();
      //EditorApplication.projectWindowChanged += OnProjectWindowChanged;
    }

    private static void ReadAxes()
    {
      axesIndex = new Dictionary<string, int>(); 
      var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
      var obj = new SerializedObject(inputManager);
      axesArray = obj.FindProperty("m_Axes");
      var axisList = new List<string>();
      for(var i = 0; i < axesArray.arraySize; ++i)
      {
        var axis = axesArray.GetArrayElementAtIndex(i);
        var name = axis.FindPropertyRelative("m_Name").stringValue;
        axisList.Add(name);

        // Don't add duplicates!
        if (!axesIndex.ContainsKey(name))
          axesIndex.Add(name, i);
      }

      InputManagerUtility.axesNames = axisList.ToArray();
    }

    //private static void OnProjectWindowChanged()
    //{
    //  Trace.Script("Change!");
    //}

    /// <summary>
    /// Gets the current 
    /// </summary>
    /// <param name="inputAxis"></param>
    /// <returns></returns>
    public static int GetIndex(string inputAxis)
    {
      // If the input axis by that name doesn't exist
      if (!axesIndex.ContainsKey(inputAxis))
        return -1;

      return axesIndex[inputAxis];
    }

  }
}