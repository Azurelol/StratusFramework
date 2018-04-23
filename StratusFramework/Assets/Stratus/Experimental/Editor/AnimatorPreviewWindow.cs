using UnityEngine;
using UnityEditor;
//using UnityEngine.Animations;
using System.Linq;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEditor.Experimental.Animations;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Stratus
{
  public class AnimatorPreviewWindow : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    public Animator sceneAnimator;
    public GameObject prefab;
    
    private GameObject animatorObject;
    private Animator animator;
    private UnityEditor.Animations.AnimatorController animatorController;
    private AnimationClip[] animationClips;
    private UnityEngine.AnimatorControllerParameter[] parameters;
    //private UnityEditor.Animations.AnimatorControllerLayer[] layers;
    private int selectedAnimationClipIndex;
    private string[] animationClipNames;
    private EditorWindow animationWindow { get; set; }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    private bool hasAnimator => animator != null;
    private bool hasAnimationClips => animationClips != null && animationClips.Length > 0;
    private AnimationClip selectedAnimation => animationClips[selectedAnimationClipIndex];

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Windows/Animator Preview")]
    private static void Open()
    {
      EditorWindow.GetWindow(typeof(AnimatorPreviewWindow), true, "Animator Preview");
    }

    private void OnEnable()
    {
      animatorObject = null;
      sceneAnimator = null;
      ResetAnimationData();
      //animationWindow = Type.GetType("UnityInternal.AnimationWindow") as EditorWindow;
      //animationWindow = getFirstEditorWindowOfType("AnimationWindow");
    }

    private void OnGUI()
    {
      SelectAnimator();
      ListAnimations();
      ShowObject();
      //ShowDetails();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void SelectAnimator()
    {
      ShowHeader("Selection");
      // In the future, show both options for either an an animator from a gameobject in a prefab animator from the scene 
      //EditorGUIUtility.labelWidth = CalculateWidth(EditorStyles.label, "Prefab") + 5f;
      EditorGUILayout.BeginHorizontal();
      {
        EditorGUI.BeginChangeCheck();
        prefab = EditorGUILayout.ObjectField("Prefab in Assets", prefab, typeof(GameObject), false) as GameObject;
        if (EditorGUI.EndChangeCheck())
        {
          animatorObject = prefab;
          CheckForAnimator();
          if (prefab != null)
            sceneAnimator = null;
        }

        EditorGUI.BeginChangeCheck();
        sceneAnimator = EditorGUILayout.ObjectField("Animator in Scene", sceneAnimator, typeof(Animator), true) as Animator;
        if (EditorGUI.EndChangeCheck())
        {
          ResetAnimationData();
          if (sceneAnimator != null)
          {
            prefab = null;
            animator = sceneAnimator;
            animatorObject = animator.gameObject;
            GetAnimationData();
          }
        }
      }
      EditorGUILayout.EndHorizontal();
    }

    private void ListAnimations()
    {
      EditorGUILayout.Separator();
      ShowHeader("Animations");

      if (!animatorController)
        return;
      EditorGUILayout.ObjectField("Controller", animatorController, typeof(RuntimeAnimatorController), false);
      ShowParameters();
      ShowLayers();     

      // Show information about the animation
      if (!hasAnimationClips)
        return;
      ShowAnimationClips();
    }

    private void ShowObject()
    {
      EditorGUILayout.Separator();
      ShowHeader("Preview");
      if (!hasAnimator)
        return;

    }
    
    private void CheckForAnimator()
    {
      ResetAnimationData();

      if (animatorObject != null)
      {
        animator = animatorObject.GetComponentInChildren<Animator>();
        if (animator == null)
        {
          string erorrMsg = $"{animatorObject.name} has no animator component!";
          Trace.Script(erorrMsg);
          EditorUtility.DisplayDialog("Missing Animator", erorrMsg, "OK");
          animatorObject = null;
        }
        else
        {
          GetAnimationData();
        }
      }
    }

    private void ShowAnimationClips()
    {
      EditorGUILayout.LabelField("Clips", EditorStyles.whiteLargeLabel);
      selectedAnimationClipIndex = GUILayout.SelectionGrid(selectedAnimationClipIndex, animationClipNames, animationClips.Length / 4);
      EditorGUILayout.LabelField($"Length: {selectedAnimation.length}");
      EditorGUILayout.LabelField($"Framerate: {selectedAnimation.frameRate}");
      EditorGUILayout.LabelField($"Is Looping: {selectedAnimation.isLooping}");
    }

    private void ShowParameters()
    {
      EditorGUILayout.LabelField($"Parameters: {parameters.Length}", EditorStyles.whiteLargeLabel);
      EditorGUILayout.BeginVertical();
      foreach (var param in parameters)
      {        
        EditorGUILayout.LabelField($"{param.name} ({param.type})");
      }      
      EditorGUILayout.EndVertical();
    }

    private void ShowLayers()
    {
      EditorGUILayout.LabelField($"Layers: {animator.layerCount}");
    }

    private void ResetAnimationData()
    {
      animator = null;
      animatorController = null;
      animationClips = null;
      animationClipNames = null;
    }

    private void GetAnimationData()
    {
      this.animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
      if (this.animatorController == null)
        return;
      
      this.parameters = animatorController.parameters;
      //this.layers = animatorController.layers;
      this.animationClips = animatorController.animationClips;
      if (this.animationClips == null)
        return; 

      
      this.animationClipNames = (from clip in this.animationClips select clip.name).ToArray();
    }

    float CalculateWidth(GUIStyle style, string text)
    {
      var size = style.CalcSize(new GUIContent(text));
      return size.x;
    }

    private void ShowHeader(string title)
    {
      EditorGUILayout.LabelField(title, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
      EditorGUILayout.Separator();
    }

    //static EditorWindow getFirstEditorWindowOfType(string type)
    //{
    //  return getEditorWindowsOfType(type).FirstOrDefault();
    //}

    //static IEnumerable<EditorWindow> getEditorWindowsOfType(EditorWindow win)
    //{
    //  return getEditorWindowsOfType(win.GetType().ToString());
    //}
    //static IEnumerable<EditorWindow> getEditorWindowsOfType(string type)
    //{
    //  return (Resources.FindObjectsOfTypeAll(typeof(EditorWindow)) as EditorWindow[]).Where(x => x.GetType().ToString() == type);
    //}

  }

}