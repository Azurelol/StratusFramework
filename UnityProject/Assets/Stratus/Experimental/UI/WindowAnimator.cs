/******************************************************************************/
/*!
@file   WindowAnimator.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using Stratus.UI;

namespace Prototype 
{
  [RequireComponent(typeof(Window))]
  [RequireComponent(typeof(Animator))]
  public class WindowAnimator : MonoBehaviour 
  {
    Animator Animator;
    public float Delay = 1.5f;

    void Awake()
    {
      Animator = GetComponent<Animator>();
      this.gameObject.Connect<Stratus.UI.Window.OpenedEvent>(this.OnWindowOpened);      
      this.gameObject.Connect<Stratus.UI.Window.ClosedEvent>(this.OnWindowClosed);
    }

    void OnWindowOpened(Stratus.UI.Window.OpenedEvent e)
    {
      Animator.SetTrigger("Open");
    }

    void OnWindowClosed(Stratus.UI.Window.ClosedEvent e)
    {
      Animator.SetTrigger("Close");
    }



  }
}
