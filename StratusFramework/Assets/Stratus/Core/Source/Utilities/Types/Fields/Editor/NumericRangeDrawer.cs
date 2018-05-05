using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(FloatRange))]
  public class FloatRangeDrawer : DualPropertyDrawer
  {
    protected override string firstProperty { get; } = nameof(FloatRange.minimum);
    protected override string secondProperty { get; } = nameof(FloatRange.maximum);
  }

  [CustomPropertyDrawer(typeof(IntegerRange))]
  public class IntegerRangeDrawer : DualPropertyDrawer
  {
    protected override string firstProperty { get; } = nameof(IntegerRange.minimum);
    protected override string secondProperty { get; } = nameof(IntegerRange.maximum);
  }

}