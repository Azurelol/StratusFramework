using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  [CreateAssetMenu(fileName = "Input Sensitive Map.asset", menuName = "Stratus/Experimental/Input Sensitive Map")]
  public abstract class InputSensitiveMap : StratusScriptable
  {
    public enum InputType
    {
      Gamepad,
      Keyboard
    }

    public class Element<T> : SerializableClass
    {
      public string label;
      public T gamepad;
      public T keyboard;
    }

    public static bool hasGamepad => Input.GetJoystickNames().Length > 0;

    public T Get<T>(List<Element<T>> elements, string label) where T : UnityEngine.Object
    {
      var element = elements.Find(x => x.label == label);

      if (element.Equals(null))
        return null;

      if (hasGamepad)
        return element.gamepad;
      return element.keyboard;
    }



    //public Sprite GetSprite(string label) => Get<SpriteElement>(sprites, label);
  }

}