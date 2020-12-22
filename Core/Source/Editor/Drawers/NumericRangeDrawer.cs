using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(StratusFloatRange))]
  public class FloatRangeDrawer : DualPropertyDrawer
  {
    protected override string firstProperty { get; } = nameof(StratusFloatRange.minimum);
    protected override string secondProperty { get; } = nameof(StratusFloatRange.maximum);
  }

  [CustomPropertyDrawer(typeof(StratusIntegerRange))]
  public class IntegerRangeDrawer : DualPropertyDrawer
  {
    protected override string firstProperty { get; } = nameof(StratusIntegerRange.minimum);
    protected override string secondProperty { get; } = nameof(StratusIntegerRange.maximum);
  }

}