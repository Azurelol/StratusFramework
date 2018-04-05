using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  //public struct InputSpriteElement<Sprite> : Element<Sprite> {}

  [CreateAssetMenu(fileName = "Input Sprite Map.asset", menuName = "Stratus/Experimental/Input Sprite Map")]
  public class InputSpriteMap : InputSensitiveMap
  {
    [Serializable]
    public class SpriteElement : Element<Sprite> { }
    public List<SpriteElement> sprites = new List<SpriteElement>();

    public Sprite GetSprite(string label)
    {
      var element = sprites.Find(x => x.label == label);
    
      if (element.Equals(null))
        return null;
    
      if (hasGamepad)
        return element.gamepad;
      return element.keyboard;
    }

    //public struct SpriteElement : Element<Sprite> { }

    //public List<Spr>

  }

}