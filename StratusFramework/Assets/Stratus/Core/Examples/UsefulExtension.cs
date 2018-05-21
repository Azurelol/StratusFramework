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

    public override void OnAwake()
    {
      Trace.Script("Usefulness awoken");
    }

    public override void OnStart()
    {
      Trace.Script("Usefulness started");
    }
  }
}