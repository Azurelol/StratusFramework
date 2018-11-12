using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
  [InitializeOnLoad]
  public static class Library
  {
    public static Type[] eventTypes { get; private set; }
    public static string[] eventTypeNames { get; private set; }

    static Library()
    {
      LookUpTypes();
    }

    static void LookUpTypes()
    {
      eventTypes = Reflection.GetSubclass<Stratus.StratusEvent>(false);
      eventTypeNames = Reflection.GetSubclassNames<Stratus.StratusEvent>(false);
    }

  }

}