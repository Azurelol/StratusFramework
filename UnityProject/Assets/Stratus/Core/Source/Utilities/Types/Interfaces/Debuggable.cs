using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Interfaces
{
  /// <summary>
  /// Interface for globally toggling debug on and off
  /// </summary>
  public interface Debuggable
  {
    void Toggle(bool toggle);
  }
}