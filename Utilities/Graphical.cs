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

namespace Stratus
{
  /// <summary>
  /// 
  /// </summary>
  public class Graphical
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
        if (duration <= 0.0f)
          graphical.color = new Color(graphical.color.r, graphical.color.g, graphical.color.b, alpha);
        else
          graphical.CrossFadeAlpha(alpha, duration, true);

      }
    }
  }

}