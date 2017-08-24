/******************************************************************************/
/*!
@file   LinkAnimation.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;

namespace Stratus   
{
  namespace UI
  {
    /// <summary>
    /// Animates the link it is attached to.
    /// </summary>
    [RequireComponent(typeof(Link))]
    [RequireComponent(typeof(Animator))]
    public class LinkAnimator : MonoBehaviour 
    {
      Animator Animator;

      void Awake()
      {
        Animator = GetComponent<Animator>();
        this.gameObject.Connect<Link.TransitionEvent>(this.OnTransitionEvent);
      }

      public void SetAnimatorController()
      {
        Animator = GetComponent<Animator>();
        Animator.runtimeAnimatorController = GetComponent<Link>().Style.Animator;
      }

      void OnTransitionEvent(Link.TransitionEvent e)
      {
        switch (e.State)
        {
          case Link.LinkState.Active:
            Animator.SetTrigger("Active");
            break;
          case Link.LinkState.Default:
            Animator.SetTrigger("Default");
            break;
          case Link.LinkState.Selected:
            Animator.SetTrigger("Selected");
            break;
          case Link.LinkState.Disabled:
            Animator.SetTrigger("Disabled");
            break;
          case Link.LinkState.Hidden:
            //Animator.SetTrigger("Active");
            break;
        }
      }

    }
  }
}
