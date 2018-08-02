using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Examples
{
  [CustomExtensionAttribute(typeof(SampleExtensibleBehaviour))]
  public class UsefulExtension : StratusBehaviour, IExtensionBehaviour
  {
    public bool useful = true;

    //public SampleExtensibleBehaviour extensible { get; set; }

    public void OnExtensibleAwake(ExtensibleBehaviour extensible)
    {
      //this.extensible = (SampleExtensibleBehaviour)extensible;
    }

    public void OnExtensibleStart()
    {
      
    }
  }
}