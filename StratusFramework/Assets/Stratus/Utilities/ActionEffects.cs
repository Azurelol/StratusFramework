/******************************************************************************/
/*!
@file   ActionEffects.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionEffects 
  */
  /**************************************************************************/
  static public class ActionEffects
  {
    static public void Flash(Renderer renderer, Color color, Boolean flashing, 
                             float duration = 1.0f, Ease ease = Ease.Linear, float delay = 0.5f)
    {
      var gameObj = renderer.gameObject;
      Trace.Script("Flashing '" + gameObj.name + "'");
      var currentColor = renderer.material.color;
      var seq = Actions.Sequence(gameObj.Actions());
      Actions.Property(seq, flashing, true, 0.0f, Ease.Linear);
      Actions.Property(seq, renderer, Color.red, duration, Ease.Linear);
      Actions.Delay(seq, delay);
      Actions.Property(seq, renderer, currentColor, duration / 2, Ease.Linear);
      Actions.Property(seq, flashing, false, 0.0f, Ease.Linear);
    }

    static public void Scale(Transform transform, Vector3 scale, Boolean scaling,
                             float duration = 1.0f, Ease ease = Ease.Linear)
    {
      var gameObj = transform.gameObject;
      Trace.Script("Scaling '" + gameObj.name + "'");
      var oldScale = transform.lossyScale;

    }

  }

}