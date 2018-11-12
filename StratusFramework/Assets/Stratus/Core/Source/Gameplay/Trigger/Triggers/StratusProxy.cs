using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Observes another object's messages
  /// </summary>
  public abstract class StratusProxy : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public bool persistent = true;
    public bool debug = false;

  }
}