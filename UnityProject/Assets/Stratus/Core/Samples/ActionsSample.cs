/******************************************************************************/
/*!
@file   ActionsSample.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   7/14/2016
@brief  A demonstration of how to compose Actions.
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace Stratus
{
  namespace Examples
  {
    public class ActionsSample : StratusBehaviour
    {
      [Header("Common Settings")]
      public float duration = 1.5f;
      public Ease ease = Ease.Linear;

      [Header("Values")]
      public Color colorValue = Color.red;
      public int integerValue = 7;
      public float floatValue = 7f;
      public Image[] imageGroup;

      public RuntimeMethodField testingMethods;

      // Property: Value type POD
      private float floatProperty { set; get; } = 1.0f;
      // Property: Value type STRUCT
      private Vector2 vector2Property { set; get; } = new Vector2();

      void Awake()
      {
        testingMethods = new RuntimeMethodField(TestActionSequence, TestActionCall, TestActionGroup);
      }

      void TestActionSequence()
      {
        float delay = 2.0f;
        int finalValue = 16;

        var seq = Actions.Sequence(this);
        Actions.Trace(seq, $"Waiting {delay} seconds");
        Actions.Delay(seq, delay);
        Actions.Trace(seq, $"Now interpolating the initial value of {nameof(integerValue)} " +
          $"from {integerValue} to {finalValue} over {duration} seconds");
        Actions.Property(seq, () => this.integerValue, finalValue, duration, Ease.Linear);
        Actions.Trace(seq, $"The final value of {nameof(integerValue)} is:");
        Actions.Call(seq, () => this.PrintValue(this.integerValue));
      }

      void TestActionGroup()
      {
        Trace.Script($"Action Group Test: Interpolating the color of {imageGroup.Length} images at the same time", this);
        var group = Actions.Group(this);
        foreach(var image in imageGroup)
          Actions.Property(group, ()=>image.color, colorValue, duration, ease);
      }

      void TestActionCall()
      {
        float boops = 6f;
        float delay = 1.5f;
        string first = null;
        string second = null;

        var seq = Actions.Sequence(this);
        // First, boop
        Actions.Call(seq, () => Boop(boops));
        // Second, wait
        Actions.Trace(seq, $"Waiting {delay} seconds");
        Actions.Delay(seq, delay);
        // Third, set the values to be used
        Actions.Call(seq, () =>
        {
          delay = 5f;
          first = "Hello";
          second = "Mundo";          
        });
        Actions.Call(seq, () => Beep(first, second, this.gameObject));
      }

      void Boop(float boopValue)
      {
        Trace.Script("Booped for '" + boopValue + "' points!", this);
      }

      void Beep(string first, string second, GameObject obj)
      {
        Trace.Script(obj + " says = " + first + " " + second);
      }

      void PrintValue(object obj)
      {
        Trace.Script(obj);
      }
    } 
  }
}
