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

        var seq = StratusActions.Sequence(this);
        StratusActions.Trace(seq, $"Waiting {delay} seconds");
        StratusActions.Delay(seq, delay);
        StratusActions.Trace(seq, $"Now interpolating the initial value of {nameof(integerValue)} " +
          $"from {integerValue} to {finalValue} over {duration} seconds");
        StratusActions.Property(seq, () => this.integerValue, finalValue, duration, Ease.Linear);
        StratusActions.Trace(seq, $"The final value of {nameof(integerValue)} is:");
        StratusActions.Call(seq, () => this.PrintValue(this.integerValue));
      }

      void TestActionGroup()
      {
        Trace.Script($"Action Group Test: Interpolating the color of {imageGroup.Length} images at the same time", this);
        var group = StratusActions.Group(this);
        foreach(var image in imageGroup)
          StratusActions.Property(group, ()=>image.color, colorValue, duration, ease);
      }

      void TestActionCall()
      {
        float boops = 6f;
        float delay = 1.5f;
        string first = null;
        string second = null;

        var seq = StratusActions.Sequence(this);
        // First, boop
        StratusActions.Call(seq, () => Boop(boops));
        // Second, wait
        StratusActions.Trace(seq, $"Waiting {delay} seconds");
        StratusActions.Delay(seq, delay);
        // Third, set the values to be used
        StratusActions.Call(seq, () =>
        {
          delay = 5f;
          first = "Hello";
          second = "Mundo";          
        });
        StratusActions.Call(seq, () => Beep(first, second, this.gameObject));
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
