/******************************************************************************/
/*!
@file   Link_Transition.cs
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
    public abstract partial class Link : MonoBehaviour
    {
      /// <summary>
      /// Transitions to another state
      /// </summary>
      /// <param name="state"></param>
      void TransitionTo(LinkState state)
      {
        this.State = state;

        // If doing an animation transition
        Animate();

        //if (Style.Type == LinkTransition.TransitionType.Animation)
        //{
        //  Animate();
        //}

        // If not applying a color tint, return
        //if (!Style.Tint)
        //  return;

        // Otherwise change its color
        switch (State)
        {
          case LinkState.Active:
            this.Tint(this.Style.ActiveColor);
            break;
          case LinkState.Default:
            this.Tint(this.Style.DefaultColor);
            break;
          case LinkState.Selected:
            this.Tint(this.Style.SelectedColor);
            break;
          case LinkState.Disabled:
            this.Tint(this.Style.DisabledColor);
            break;
          case LinkState.Hidden:
            this.Tint(Color.clear);
            break;
        }


      }


      /// <summary>
      /// Shows or hides this link.
      /// </summary>
      /// <param name="show"></param>
      /// <param name="immediate">Whether this should be done immediately. </param>
      public void Show(bool show, bool immediate = false)
      {
        //Trace.Script(show, this);
        // Show
        if (show)
        {
          this.IsVisible = true;
          if (Button)
          {
            if (immediate)
              Button.CrossFadeAlpha(1.0f, 0.0f, true);
            else
              Button.CrossFadeAlpha(1.0f, this.Style.Duration, true);
          }
          if (Text)
          {
            if (immediate)
              Text.CrossFadeAlpha(1.0f, 0.0f, true);
            else
              Text.CrossFadeAlpha(1.0f, this.Style.Duration, true);
          }

          //Trace.Script("Displaying", this);
        }
        // Hide
        else
        {
          this.IsVisible = false;
          if (Button)
          {
            if (immediate)
              Button.CrossFadeAlpha(0.0f, 0.0f, true);
            else
              Button.CrossFadeAlpha(0.0f, this.Style.Duration, true);
          }
          if (Text)
          {
            if (immediate)
              Text.CrossFadeAlpha(0.0f, 0.0f, true);
            else
              Text.CrossFadeAlpha(0.0f, this.Style.Duration, true);
          }
        }
      }

      /// <summary>
      /// Animates this link, depending on its state
      /// </summary>
      void Animate()
      {
        if (!UnityEngine.Application.isPlaying)
        {
          return;
        }

        //Trace.Script("Animating link!");
        var transition = new TransitionEvent();
        transition.State = this.State;
        this.gameObject.Dispatch<TransitionEvent>(transition);
      }

      /// <summary>
      /// Changes the color of this link.
      /// </summary>
      /// <param name="enabled"></param>
      void Tint(Color color)
      {
        this.Style.Current = color;
        //Trace.Script("Applying color:" + color, this);

        // If the link is not supposed to be visible, do nothing
        // This will prevent clashes such as this overriding a crossfadealpha
        if (!IsVisible)
          return;

        if (Button)
        {
          if (UnityEngine.Application.isPlaying)
            Button.CrossFadeColor(color, this.Style.Duration, true, true);
          else
            Button.color = color;
        }
      }

    } 
  }
}
