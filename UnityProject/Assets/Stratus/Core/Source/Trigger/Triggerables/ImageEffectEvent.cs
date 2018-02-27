using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus
{
  /// <summary>
  /// Affects an image
  /// </summary>
  public class ImageEffectEvent : Triggerable
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public enum Type
    {
      [Tooltip("Fades the alpha associated with the graphic")]
      FadeAlpha,
      [Tooltip("Fades the color of the image")]
      FadeColor
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Header("Image Settings")]
    public Image image;
    [Tooltip("What type of image effect to apply")]
    public Type type = Type.FadeAlpha;
    // Fade Alpha 
    [DrawIf("type", Type.FadeAlpha, ComparisonType.Equals)]
    public float alpha = 0.0f;
    // Fade Color
    [DrawIf("type", Type.FadeColor, ComparisonType.Equals)]
    public Color color = Color.white;
    [DrawIf("type", Type.FadeColor, ComparisonType.Equals)]
    public bool useAlpha = true;
    [Header("Common Settings")]
    // Common settings
    [Tooltip("Duration of the transition in seconds")]
    public float duration = 1.5f;
    [Tooltip("Should it ignore the time scale?")]
    public bool ignoreTimeScale = false;
    [Tooltip("Whether to revert to the original value on a subsequent trigger")]
    public bool toggle;

    private float previousAlpha, currentAlpha;
    private Color previousColor, currentColor;
    private bool isFaded => currentAlpha == 0.0f;
    private bool isColored => currentColor == color;
    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      currentAlpha = image.color.a;
      currentColor = image.color;
    }

    protected override void OnTrigger()
    {
      switch (type)
      {
        case Type.FadeAlpha:
          if (toggle)
            Fade(isFaded ? previousAlpha : 0.0f, duration, ignoreTimeScale);
          else 
            Fade(alpha, duration, ignoreTimeScale);
          break;
        case Type.FadeColor:
          if (toggle)
            FadeColor(isColored ? previousColor : color, duration, ignoreTimeScale);
          else
            FadeColor(color, duration, ignoreTimeScale);
          break;
        default:
          break;
      }
    }

    protected override void OnReset()
    {

    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public void Fade(float alpha, float duration, bool ignoreTimeScale)
    {
      if (logging)
        Trace.Script("Fading to " + alpha, this);
      previousAlpha = currentAlpha;
      currentAlpha = alpha;
      //image.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
      this.StartCoroutine(Routines.Lerp(image.color.a, alpha, duration, (float val) => { image.color = image.color.SetAlpha(val); }, Routines.Lerp), "Fade");      
      
    }

    public void FadeColor(Color color, float duration, bool ignoreTimeScale)
    {
      if (logging)
        Trace.Script("Fading to " + color, this);
      previousColor = currentColor;
      currentColor = color;
      //image.CrossFadeColor(color, duration, ignoreTimeScale, useAlpha);
      this.StartCoroutine(Routines.Lerp(image.color, color, duration, (Color val) => { image.color = val; }, Color.Lerp), "Fade Color");
    }

    public void SetAlpha(float alpha)
    {
      image.color = image.color.SetAlpha(alpha);
    }

  }

}