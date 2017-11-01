/******************************************************************************/
/*!
@file   AnimateEvent.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Stratus 
{
  public class AnimateEvent : Triggerable
  {
    public Animator Animator;
    public string Animation;

    protected override void OnAwake()
    {
      
    }

    protected override void OnTrigger()
    {
      Animator.SetTrigger(this.Animation);
    }
  }
}
