using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Stratus
{
  public class InputModule : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/


    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public List<InputAction> inputs = new List<InputAction>();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {

    }

    private void Update()
    {
      foreach (var input in inputs)
      {
        Update();
      }
    }
  }



}