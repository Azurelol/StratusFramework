using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public struct BooleanOptionField
  {
    public enum Value
    {
      Default,
      True,
      False
    }

    public Value value;
  }

}