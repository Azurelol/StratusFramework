/******************************************************************************/
/*!
@file   RendererRoutines.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
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

  }

}
