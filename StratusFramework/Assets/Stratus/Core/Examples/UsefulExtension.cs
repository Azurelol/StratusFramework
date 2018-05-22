using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Examples
{
  [CustomExtension(typeof(SampleExtensibleBehaviour))]
  public class UsefulExtension : ExtensionBehaviour
  {
    public bool useful = true;

    protected override void OnExtensibleAwake()
    {
      Trace.Script("Usefulness awoken");
    }

    protected override void OnExtensibleStart()
    {
      Trace.Script("Usefulness started");
    }

  }
}