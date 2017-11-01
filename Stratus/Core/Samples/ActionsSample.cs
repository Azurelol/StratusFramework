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

namespace Stratus
{
  namespace Examples
  {
    public class ActionsSample : MonoBehaviour
    {
      public RuntimeMethodField testSequence;
      public RuntimeMethodField testCallAdvanced;

      // Field: Custom reference-type
      private int integerField = 5;
      // Property: Value type POD
      private float floatProperty { set; get; } = 1.0f;
      // Property: Value type STRUCT
      private Vector2 vector2Property { set; get; } = new Vector2();

      void Start()
      {
        testSequence = new RuntimeMethodField(TestSequence);
        testCallAdvanced = new RuntimeMethodField(TestCallAdvanced);
      }

      void TestSequence()
      {
        float delay = 2.0f;
        float duration = 3.0f;
        int finalValue = 16;

        var seq = Actions.Sequence(this);
        Actions.Trace(seq, $"Waiting {delay} seconds");
        Actions.Delay(seq, delay);
        Actions.Trace(seq, $"Now interpolating the initial value of {nameof(integerField)} " +
          $"from {integerField} to {finalValue} over {duration} seconds");
        Actions.Property(seq, () => this.integerField, finalValue, duration, Ease.Linear);
        Actions.Trace(seq, $"The final value of {nameof(integerField)} is:");
        Actions.Call(seq, () => this.PrintValue(this.integerField));
      }

      void TestCallAdvanced()
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
