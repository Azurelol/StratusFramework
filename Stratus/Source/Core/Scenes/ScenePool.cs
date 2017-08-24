using UnityEngine;
using System;

namespace Stratus
{
  namespace Experimental
  {
    [CreateAssetMenu(fileName = "Scene Pool", menuName = "Stratus/Scene Pool")]
    public class ScenePool : ScriptableObject
    {
      /// <summary>
      /// The list of scenes being used in the project
      /// </summary>
      public SceneField[] scenes;

      /// <summary>
      /// The initial scene to be loaded upon entering play mode (will be the first one listed on the scenes array)
      /// </summary>
      public SceneField initialScene => scenes[0];

      /// <summary>
      /// Opens all scenes in the editor
      /// </summary>
      public void OpenAll()
      {
        foreach (var scene in this.scenes)
          scene.Add();
      }

      /// <summary>
      /// Closes all scenes in the editor
      /// </summary>
      public void CloseAll()
      {
        foreach (var scene in this.scenes)
          scene.Close();
      }

      [Serializable]
      public class Selection
      {

      }

    }  

    

  }
}
