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
      public SceneField initialScene
      {
        get
        {
          if (scenes.Length > 0)
            return scenes[0];
          return null;
        }
      } 

      /// <summary>
      /// Opens all scenes in the editor
      /// </summary>
      public void OpenAll(Scene.SceneCallback onOpened = null)
      {
        Scene.Load(scenes, onOpened);

        //foreach (var scene in this.scenes)
        //  scene.Add();
      }

      /// <summary>
      /// Closes all scenes in the editor
      /// </summary>
      public void CloseAll(Scene.SceneCallback onClosed = null)
      {
        Scene.Unload(scenes, onClosed);

        //foreach (var scene in this.scenes)
        //  scene.Close();


      }

    }  

    

  }
}
