/******************************************************************************/
/*!
@file   Graphical.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System.Collections;

namespace Stratus
{
  namespace Utilities
  {
    public static partial class Graphical
    {
      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Tweens the alpha of this and all its children.
      /// </summary>
      /// <param name="alpha"></param>
      /// <param name="duration"></param>
      public static void Fade(MonoBehaviour target, float alpha, float duration)
      {
        foreach (var graphical in target.GetComponentsInChildren<Graphic>())
        {
          //Trace.Script("Fading '" + graphical.name + " to " + alpha);
          if (duration <= 0.0f)
            graphical.color = new Color(graphical.color.r, graphical.color.g, graphical.color.b, alpha);
          else
            graphical.CrossFadeAlpha(alpha, duration, true);

        }
      }

      /// <summary>
      /// Tweens the alpha of this and all its children after initializing it to 0.
      /// </summary>
      /// <param name="target"></param>
      /// <param name="alpha"></param>
      /// <param name="duration"></param>
      public static void FadeIn(MonoBehaviour target, float alpha, float duration)
      {
        target.StartCoroutine(FadeRoutine(target, 0.0f, 0.0f));
        target.StartCoroutine(FadeRoutine(target, alpha, duration));
      }

      /// <summary>
      /// Makes a color struct from a given hex value
      /// </summary>
      /// <param name="hex">The hex value</param>
      /// <returns></returns>
      public static Color HexToColor(string hex)
      {
        var color = new Color();
        // In case the string is formated 0xFFFFFF
        hex = hex.Replace("0x", "");
        // In case the string is formated #FFFFFF
        hex = hex.Replace("#", "");
        // Assume fully visible unless specified in hex
        color.r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        color.g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        color.b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        // Only use alpha if string has enough characters
        if (hex.Length == 8)
          color.a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        else
          color.a = 1;
        
        return color;
      }

      static IEnumerator FadeRoutine(MonoBehaviour target, float alpha, float duration)
      {
        yield return new WaitForEndOfFrame();
        Fade(target, alpha, duration);
        yield return null;
      }

      /// <summary>
      /// Gets the visible boundaries of this transform
      /// </summary>
      /// <param name="source"></param>
      /// <returns></returns>
      public static Bounds GetVisibleBoundaries(Transform source)
      {
        var renderers = source.GetComponentsInChildren<Renderer>(true);
        bool renderersPresent = renderers.Length != 0;

        if (!renderersPresent)
          return new Bounds();

        Renderer firstRenderer = renderers[0];
        Bounds totalBounds = firstRenderer.bounds;
        for (var i = 1; i < renderers.Length; ++i)
        {
          var renderer = renderers[i];
          var bounds = renderer.bounds;
          totalBounds.Encapsulate(bounds);
        }

        return totalBounds;
      }

    } 
  }

}