using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Examples
{
  [StratusCustomExtensionAttribute(typeof(SampleExtensibleBehaviour))]
  public class UsefulExtension : StratusBehaviour, IStratusExtensionBehaviour
  {
    public bool useful = true;

    //public SampleExtensibleBehaviour extensible { get; set; }

    public void OnExtensibleAwake(StratusExtensibleBehaviour extensible)
    {
      //this.extensible = (SampleExtensibleBehaviour)extensible;
    }

    public void OnExtensibleStart()
    {
      
    }
  }
}