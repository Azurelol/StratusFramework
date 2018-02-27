using System.Collections;
using System.Collections.Generic;
using System;

namespace Stratus
{
  /// <summary>
  /// Provides useful functions and a generic property drawer that can handle 
  /// multiple fields, etc. Make sure to still use the [Serializable] attribute!
  /// </summary>
  [Serializable]
  public abstract class SerializableClass
  {
    public int fieldCount { get; private set; }

    public SerializableClass()
    {
      Type declaredType = GetType().DeclaringType;
      fieldCount = declaredType.GetFields().Length;
    }

  }


}