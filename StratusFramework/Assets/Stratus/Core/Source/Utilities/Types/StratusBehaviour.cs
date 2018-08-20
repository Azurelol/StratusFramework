using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;

namespace Stratus
{
  /// <summary>
  /// Base class for MonoBehaviours that use Stratus's custom editors for components,
  /// and handles custom serialization (through Sirenix's Odin Serializer)
  /// </summary>
  public abstract class StratusBehaviour : SerializedMonoBehaviour
  {
  }

}