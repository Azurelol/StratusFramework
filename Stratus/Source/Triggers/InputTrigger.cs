/******************************************************************************/
/*!
@file   InputTrigger.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype 
{
  public class InputTrigger : EventTrigger
  {
    public KeyCode Key = KeyCode.A;

    protected override void OnEnabled()
    {
      
    }

    protected override void OnInitialize()
    {
      
    }

    void Update()
    {
      if (Input.GetKeyDown(this.Key))
      {
        this.Trigger();
      }
    }
  }
}
