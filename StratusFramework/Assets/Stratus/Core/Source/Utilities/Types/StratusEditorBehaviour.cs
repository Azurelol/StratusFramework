using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// An editor-only behaviour, used for utility
  /// </summary>
  [DisallowMultipleComponent]
  public class StratusEditorBehaviour<T> : Multiton<T> where T : MonoBehaviour
  {
    protected override void OnAwake()
    {      
    }

    protected override void OnMultitonDisable()
    {      
    }

    protected override void OnMultitonEnable()
    {      
    }

    protected override void OnReset()
    {      
    }

  }

}