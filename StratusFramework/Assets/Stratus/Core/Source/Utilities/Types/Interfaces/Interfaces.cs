using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace Stratus
{
  /// <summary>
  /// A collection of useful interfaces for development
  /// </summary>
  namespace Interfaces
  {
    //------------------------------------------------------------------------/
    // Global Controls
    //------------------------------------------------------------------------/
    /// <summary>
    /// Provides global access to interfaces
    /// </summary>
    public static class Global
    {
      /// <summary>
      /// Toggle for all behaviours inheriting from DebugToggle
      /// </summary>
      /// <param name="toggle"></param>
      public static void DebugToggle(bool toggle)
      {
        Debuggable[] toggles = FindInterfaces<Debuggable>();
        foreach (var t in toggles)
          t.Toggle(toggle);

        Trace.Script($"Debug = {toggle} on {toggles.Length} behaviours.");
      }

      /// <summary>
      /// Toggle for specific behaviours implementing DebugToggle
      /// </summary>
      /// <param name="toggle"></param>
      public static void DebugToggle<T>(bool toggle) where T : Debuggable
      {
        T[] toggles = FindInterfaces<Debuggable>().OfType<T>().ToArray();
        foreach (var t in toggles)
          t.Toggle(toggle);

        Trace.Script($"Debug = {toggle} on {toggles.Length} behaviours.");
      }

      /// <summary>
      /// Validates all loaded behaviours that implement Validator
      /// </summary>
      /// <param name="toggle"></param>
      public static Validation[] Validate()
      {
        Validator[] validators = FindInterfaces<Validator>();
        var messages = Validation.Aggregate(validators);
        Trace.Script($"Validated {validators.Length} behaviours.");
        return messages.ToArray();

        // Open a window here ..
      }

      /// <summary>
      /// Validates all loaded behaviours that implement Validator
      /// </summary>
      /// <param name="toggle"></param>
      public static Validation[] ValidateAggregate()
      {
        ValidatorAggregator[] validators = FindInterfaces<ValidatorAggregator>();
        var messages = Validation.Aggregate(validators);
        Trace.Script($"Validated {validators.Length} behaviours.");
        return messages.ToArray();
        // Open a window here ..
      }

      /// <summary>
      /// Finds any loaded components that implement the specified interfaces 
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns></returns>
      public static T[] FindInterfaces<T>() where T : class
      {
        return MonoBehaviour.FindObjectsOfType<MonoBehaviour>().OfType<T>().ToArray();
        //return MonoBehaviour.FindObjectsOfType<MonoBehaviour>().Select(c => c as T).Where(c => c != null).ToArray();
      }

      ///// <summary>
      ///// Finds any loaded components that implement the specified interfaces 
      ///// </summary>
      ///// <typeparam name="T"></typeparam>
      ///// <returns></returns>
      //public static MonoBehaviour[] FindInterfaces(Type type)
      //{
      //  return MonoBehaviour.FindObjectsOfType<MonoBehaviour>().OfType(type).ToArray();
      //  //return MonoBehaviour.FindObjectsOfType<MonoBehaviour>().Select(c => c as T).Where(c => c != null).ToArray();
      //}
    }




  }

}