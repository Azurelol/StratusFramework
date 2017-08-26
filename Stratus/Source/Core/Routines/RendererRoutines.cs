/******************************************************************************/
/*!
@file   RendererRoutines.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  public static partial class Routines
  {

    public static IEnumerator Fade(Renderer[] renderers, float alpha, float duration, bool setActive)
    {
      //Trace.Script("Fading out");
      foreach (var renderer in renderers)
      {
        //if (renderer.material.color == null)
        //  continue;

        //Trace.Script("Fading out" + renderer.name);
        float diffAlpha = (alpha - renderer.material.color.a);

        float counter = 0f;
        while (counter < duration)
        {
          float alphaAmount = renderer.material.color.a + (Time.deltaTime * diffAlpha) / duration;
          renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alphaAmount);
          counter += Time.deltaTime;
          yield return null;
        }
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
        if (setActive)
        {
          renderer.transform.gameObject.SetActive(setActive);
        }
      }
    }

    public static IEnumerator Fade(this GameObject go, float alpha, float duration, bool setActive = true)
    {
      Renderer sr = go.GetComponent<Renderer>();
      float diffAlpha = (alpha - sr.material.color.a);

      float counter = 0;
      while (counter < duration)
      {
        float alphaAmount = sr.material.color.a + (Time.deltaTime * diffAlpha) / duration;
        sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, alphaAmount);

        counter += Time.deltaTime;
        yield return null;
      }
      sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, alpha);
      if (setActive)
      {
        sr.transform.gameObject.SetActive(setActive);
      }
    }

    public static IEnumerator Fade(Light light, Color color, float range, float intensity, float duration, TimeScale timeScale = TimeScale.Delta)
    {
      Color startColor = light.color;
      float startRange = light.range;
      float startIntensity = light.intensity;

      System.Action<float> func = (float t) =>
      {
        light.color = Color.Lerp(startColor, color, t);
        light.range = Lerp(startRange, range, t);
        light.intensity = Lerp(startIntensity, intensity, t);
        //Trace.Script($" color = {light.color} range = {light.range} intensity = {light.intensity}");
      };

      yield return Lerp(func, duration, timeScale);
    }

    /// <summary>
    /// Commonly used for alpha blending
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float Lerp(float a, float b, float t)
    {
      return (1 - t) * a + t * b;
    }

  }

}
