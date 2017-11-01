/******************************************************************************/
/*!
@file   LinkInterface_Routines.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System.Collections;

namespace Stratus
{
  namespace UI
  {
    public abstract partial class LinkInterface : MonoBehaviour
    {
      /// <summary>
      /// Switches to a new link layer.
      /// </summary>
      /// <param name="layer">The layer which to switch to. </param>
      /// <param name="resetState">Whether to reset to the default state of this layer.</param>
      public void SwitchLayer(LinkLayer layer, bool resetState = true)
      {
        if (!Controller.SwitchLayer(layer))
          return;

        // If resetting the state..
        ResetLayer(resetState);
      }

      /// <summary>
      /// Switches to a new link layer.
      /// </summary>
      /// <param name="layerName">The name layer which to switch to. </param>
      /// <param name="resetState">Whether to reset to the default state of this layer.</param>
      public void SwitchLayer(string layerName, bool resetState = true)
      {
        if (!Controller.SwitchLayer(layerName))
          return;

        ResetLayer(resetState);
      }

      /// <summary>
      /// Switches to the previous link layer.
      /// </summary>
      /// <param name="resetState"></param>
      public void SwitchLayerToPrevious(bool resetState = true)
      {
        if (!Controller.SwitchLayerToPrevious())
          return;

        ResetLayer(resetState);
      }

      /// <summary>
      /// Resets the state of the current layer.
      /// </summary>
      /// <param name="resetState"></param>
      void ResetLayer(bool resetState)
      {
        // Select the first link
        if (resetState)
        {
          CurrentLayer.Reset();
          SelectFirstLink();
        }
        // Otherwise, go back to the previous state
        else
        {
          this.CurrentLink = CurrentLayer.Restore();
        }
      }



      /// <summary>
      /// Selects the first available link among all links.
      /// </summary>
      protected void SelectFirstLink()
      {
        if (Controller)
        {
          CurrentLink = Controller.SelectFirstLink();
        }

        // If there's a link available, select it
        if (CurrentLink)
        {
          CurrentLink.Select();
        }
        //Trace.Script("Selecting link!");
      }

      /// <summary>
      /// Selects the specified link.
      /// </summary>
      /// <param name="link"></param>
      protected void SelectLink(Link link)
      {
        this.CurrentLink.Deselect();
        this.CurrentLink = link;
        this.CurrentLink.Select();
      }

      /// <summary>
      /// Deselects the current link.
      /// </summary>
      protected void DeselectLink()
      {
        //Trace.Script("Deselecting link!");
        this.CurrentLink = null;
      }

      /// <summary>
      /// Selects, then opens the specified link.
      /// </summary>
      /// <param name="link"></param>
      protected void OpenLink(Link link)
      {
        //Trace.Script("Opening " + link.name, this);
        this.CurrentLink = link;
        this.CurrentLink.Select();
        CurrentLink.gameObject.Dispatch<Link.ConfirmEvent>(new Link.ConfirmEvent());
      }

      /// <summary>
      /// Selects, then opens the specified link.
      /// </summary>
      /// <param name="name">The name of the link.</param>
      protected void OpenLink(string name)
      {
        var link = Controller.Find(name);



        SelectLink(link);
        link.Confirm();
      }

      /// <summary>
      /// Toggles the LinkInterface on and off.
      /// </summary>
      /// <param name="toggle"></param>
      public void Toggle(bool toggle)
      {
        Active = toggle;
        ToggleGraphics(toggle);
        ToggleControls(toggle);
        DisableInput(this.InputDelay);
      }

      /// <summary>
      /// Toggles graphics for the LinkInterface.
      /// </summary>
      /// <param name="showing"></param>
      public void ToggleGraphics(bool showing)
      {
        //if (showing)
        //  Graphical.Fade(this, 1.0f, 0.0f);
        //else
        //  Graphical.Fade(this, 0.0f, 0.0f);

        foreach (var graphical in GetComponentsInChildren<Graphic>())
        {
          //Trace.Script("Showing" + graphical.name, this);
          graphical.enabled = showing;
        }
      }

      /// <summary>
      /// Toggles input for the LinkInterface.
      /// </summary>
      /// <param name="enabled"></param>
      public void ToggleControls(bool enabled, bool nextFrame = false)
      {
        if (Tracing) Trace.Script("Toggle = " + enabled, this);
        this.gameObject.Dispatch<ToggleInputEvent>(new ToggleInputEvent(enabled), nextFrame);
      }

      /// <summary>
      /// Disables input for a specified duration.
      /// </summary>
      /// <param name="duration">How long should input be disabled for.</param>
      public void DisableInput(float duration = 0.0f)
      {

        if (duration == 0.0f)
        {
          this._IsAcceptingInput = true;
          return;
        }

        StartCoroutine(this.DisableInputRoutine(duration));
      }

      public IEnumerator DisableInputRoutine(float duration = 0.0f)
      {
        this._IsAcceptingInput = false;
        //Trace.Script("Disabling input reception (?) for " + duration + " seconds!", this);
        yield return StartCoroutine(Routines.WaitForRealSeconds(duration));
        this._IsAcceptingInput = true;
        //Trace.Script("Input is now being received", this);
      }

      public void EnableLinks(bool enabled)
      {
        Controller.Enable(enabled);
      }
    }

  }
}