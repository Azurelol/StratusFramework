using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Stratus.Experimental
{
  [Singleton("Screen Transition", true, false)]
  //[RequireComponent(typeof(Camera))]
  public class ScreenTransition : Singleton<ScreenTransition>
  {
    //------------------------------------------------------------------------/
    // Event
    //------------------------------------------------------------------------/
    [Serializable]
    public class TransitionEvent : Stratus.Event
    {
      /// <summary>
      /// The mask being used for this transition
      /// </summary>
      [Tooltip("The mask being used for this transition")]
      public Texture2D mask;
      /// <summary>
      /// What to set the initial value at.
      /// </summary>
      [Tooltip("What to set the initial value at")]
      [Range(0f, 1f)]
      public float initialValue = 1.0f;
      /// <summary>
      /// The final value.
      /// </summary>
      [Tooltip("What to set the ending value at")]
      [Range(0f, 1f)]
      public float endingValue = 0.0f;
      /// <summary>
      /// How fast the transition takes to complete
      /// </summary>
      [Tooltip("How fast the transition takes to complete")]
      [Range(1f, 5f)]
      public float speed = 3.0f;
      /// <summary>
      /// How long the transition lasts.
      /// </summary>
      [Tooltip("How long the transition effect lasts")]
      [Range(0f, 10f)]
      public float duration = 0.0f;

      public TransitionEvent(Texture2D mask, float initialValue, float endingValue, float speed, float duration)
      {
        this.mask = mask;
        this.initialValue = initialValue;
        this.endingValue = endingValue;
        this.speed = speed;
        this.duration = duration;
      }

      public TransitionEvent()
      {
      }

    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// Provides a shader property that is set in the inspector
    /// and a material instantiated from the shader
    //public Camera target;
    public Shader shader;
    [Range(0, 1.0f)]
    public float maskValue;
    public Color maskColor = Color.black;
    public Texture2D maskTexture;
    public bool maskInvert;
    private Material m_Material;
    private bool m_maskInvert;
    private ActionSet currentSeq;

    /// <summary>
    /// The current material being used by the shader
    /// </summary>
    Material material
    {
      get
      {
        if (m_Material == null)
        {
          m_Material = new Material(shader);
          m_Material.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_Material;
      }
    }

    //public bool Active;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the CameraTransition component.
    /// </summary>
    protected override void OnAwake()
    {
      // Disable if we don't support image effects
      if (!SystemInfo.supportsImageEffects)
      {
        enabled = false;
        return;
      }

      // Subscribe to events
      Scene.Connect<TransitionEvent>(this.OnTransitionEvent);
    }
    
    void OnDisable()
    {
      if (m_Material)
      {
        DestroyImmediate(m_Material);
      }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!enabled)
      {
        Graphics.Blit(source, destination);
        return;
      }

      material.SetColor("_MaskColor", maskColor);
      material.SetFloat("_MaskValue", maskValue);
      material.SetTexture("_MainTex", source);
      material.SetTexture("_MaskTex", maskTexture);

      if (material.IsKeywordEnabled("INVERT_MASK") != maskInvert)
      {
        if (maskInvert)
          material.EnableKeyword("INVERT_MASK");
        else
          material.DisableKeyword("INVERT_MASK");
      }

      Graphics.Blit(source, destination, material);
    }

    void OnTransitionEvent(TransitionEvent e)
    {
      // If there's a mask, change to it
      if (e.mask) this.maskTexture = e.mask;
      // Set the initial value instantly
      maskValue = e.initialValue;
      // Create a sequence for the transition
      currentSeq?.Cancel();
      currentSeq = Actions.Sequence(this);
      Actions.Property(currentSeq, () => maskValue, e.endingValue, e.speed, Ease.Linear);

      // If the transition is of a fixed duration
      if (e.duration > 0.0f)
      {
        Actions.Delay(currentSeq, e.duration);
        Actions.Property(currentSeq, () => maskValue, e.initialValue, e.speed, Ease.Linear);
      }
    }

    public static void Transition(TransitionEvent e)
    {
      Scene.Dispatch<ScreenTransition.TransitionEvent>(e);
    }

  }

}