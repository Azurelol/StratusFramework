using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  namespace Examples
  {
    /// <summary>
    /// A simple component showcasing the most useful types provided by the Stratus library
    /// </summary>
    public class TypesExample : StratusBehaviour
    {
      [Header("Stratus Field Types")]
      public StratusInputBinding inputField = new StratusInputBinding();
      public StratusSceneField scene = new StratusSceneField();
      public StratusTagField tagField = new StratusTagField();
      public StratusFloatRange floatRange = new StratusFloatRange();
      public StratusIntegerRange intRange = new StratusIntegerRange();
      public StratusVariableAttribute variable = new StratusVariableAttribute();
      public KeyCode enumDrawer;

      public StratusLayerField layer = new StratusLayerField(); 

      void TryReadingInput()
      {
        var value = inputField.value;
        if (inputField.isPositive) {}
        if (inputField.isNegative) {}
        if (inputField.isNeutral) {}
        StratusDebug.Log(value);        
      }

      void TryLoadingScene()
      {
        scene.Load(UnityEngine.SceneManagement.LoadSceneMode.Single);
      }

      void TryTag()
      {        
        if (gameObject.CompareTag(tagField))
          StratusDebug.Log("The GameObject's tag and selected tag field match! (" + tagField + ")");
      }

      [StratusInvokeMethodAttribute( "TryLayer")]
      void TryLayer()
      {
        if (layer.Matches(this.gameObject))
          StratusDebug.Log("The GameObject's layer and selected layer field are a match! (" + layer + ")");
      }     
      

    } 
  }

}