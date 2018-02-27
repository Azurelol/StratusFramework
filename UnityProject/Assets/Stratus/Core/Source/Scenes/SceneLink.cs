using UnityEngine;
using Stratus;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace Experimental
  {
    /// <summary>
    /// When triggered, makes sure the selected scenes are loaded
    /// </summary>
    public class SceneLink : Triggerable
    {
      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      [Tooltip("What pool of scenes are being considered by this link")]
      public ScenePool scenePool;

      [Tooltip("What scenes should be loaded when this link is triggered.")]
      [HideInInspector]
      public List<SceneField> selectedScenes = new List<SceneField>();

      //----------------------------------------------------------------------/
      // Properties: Public
      //----------------------------------------------------------------------/
      /// <summary>
      /// The last link visited
      /// </summary>
      public static SceneLink lastVisited { private set; get; }
      /// <summary>
      /// All currently enabled links
      /// </summary>
      public static SceneLink[] activeLinks => activeLinksList.ToArray();

      //----------------------------------------------------------------------/
      // Fields: Private
      //----------------------------------------------------------------------/
      private static List<SceneLink> activeLinksList = new List<SceneLink>();

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      protected override void OnAwake()
      {
        if (SceneLinker.get == null)
          throw new Exception("HA!");
      }

      protected override void OnTrigger()
      {
        if (lastVisited != this)
        {
          LoadScenes();
          lastVisited = this;
        }
      }

      private void OnEnable()
      {
        activeLinksList.Add(this);
      }

      private void OnDisable()
      {
        activeLinksList.Remove(this);
      }

      protected override void OnReset()
      {

      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Makes sure all scenes specified are loaded when this link is triggered,
      /// unloading all other ones not listed
      /// </summary>
      void LoadScenes()
      {
        var lut = new Dictionary<string, SceneField>();
        foreach (var scene in selectedScenes)
          lut.Add(scene.name, scene);
        
        foreach(var scene in Scene.activeScenes)
        {
          // Do not unload the master (active scene)
          if (scene.isActiveScene)
            continue;

          if (!lut.ContainsKey(scene.name))
          {
            scene.Unload();
          }          
        }

        foreach(var scene in selectedScenes)
        {
          if (!scene.isLoaded)
          {
            Trace.Script("Loading " + scene);
            scene.Load(LoadSceneMode.Additive);
          }
        }

      }


    } 
  }

}