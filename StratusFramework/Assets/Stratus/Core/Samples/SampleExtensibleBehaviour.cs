using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Examples
{
  public class SampleExtensibleBehaviour : ExtensibleBehaviour
  {
    public int usefulValue = 7;

    [CustomExtension(typeof(SampleExtensibleBehaviour))]
    public class UsefulExtension : Extension
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


    protected override void OnAwake()
    {
      
    }

    protected override void OnStart()
    {
      
    }

  }

}