using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Displays an improved selected for enumerations that contains a search field
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public class SearchableEnumAttribute : PropertyAttribute
  {
  }
}