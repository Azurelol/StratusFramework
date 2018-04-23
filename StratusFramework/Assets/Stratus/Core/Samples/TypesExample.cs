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
    public class TypesExample : MonoBehaviour
    {
      [Header("Useful Editor Types")]
      public InputAxisField inputAxis = new InputAxisField();
      public InputField inputField = new InputField();
      public SceneField scene = new SceneField();
      public TagField tagField = new TagField();

      [InvokeMethodButton(typeof(TypesExample), "TryLayer")]
      public LayerField layer = new LayerField(); 

      void TryReadingInput()
      {
        var value = Input.GetAxis(inputAxis);
        if (inputAxis.isPositive) {}
        if (inputAxis.isNegative) {}
        if (inputAxis.isNeutral) {}
        Trace.Script(value);        
      }

      void TryLoadingScene()
      {
        scene.Load(UnityEngine.SceneManagement.LoadSceneMode.Single);
      }

      void TryTag()
      {        
        if (gameObject.CompareTag(tagField))
          Trace.Script("The GameObject's tag and selected tag field match! (" + tagField + ")");
      }

      void TryLayer()
      {
        if (layer.Matches(this.gameObject))
          Trace.Script("The GameObject's layer and selected layer field are a match! (" + layer + ")");
      }     
      

    } 
  }

}