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
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    public static string[] axesNames { get; private set; }
    public static Dictionary<string, int> axesIndex { get; private set; }
    public static SerializedObject inputManagerObject { get; private set; }
    public static SerializedProperty inputManagerAxesProperty { get; private set; }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    private const string inputManagerPath = "ProjectSettings/InputManager.asset";

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/  
    static InputManagerUtility()
    {
      Object inputManager = AssetDatabase.LoadAllAssetsAtPath(InputManagerUtility.inputManagerPath)[0];
      inputManagerObject = new SerializedObject(inputManager);
      inputManagerAxesProperty = inputManagerObject.FindProperty("m_Axes");

      StratusAssetProcessor.ObjectModified(inputManager, OnInputManagerChanged);

      ReadAxes();
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/  
    private static void OnInputManagerChanged(Object target)
    {
      ReadAxes();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Given an axis by string, returns the index of it within the InputManager's axes array.
    /// Returns -1 if it could not be found.
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
    private static void ReadAxes()
    {      
      ReadAxes(inputManagerObject);
    }

    private static bool IsModified()
    {      
      return false;
    }

    private static void ReadAxes(SerializedObject inputManagerSerializedObject)
    {
      axesIndex = new Dictionary<string, int>();
      var axisList = new List<string>();
      for (var i = 0; i < inputManagerAxesProperty.arraySize; ++i)
      {
        var axis = inputManagerAxesProperty.GetArrayElementAtIndex(i);
        var name = axis.FindPropertyRelative("m_Name").stringValue;
        axisList.Add(name);

        // Don't add duplicates!
        if (!axesIndex.ContainsKey(name))
          axesIndex.Add(name, i);
      }

      InputManagerUtility.axesNames = axisList.ToArray();
    }
    



  }
}