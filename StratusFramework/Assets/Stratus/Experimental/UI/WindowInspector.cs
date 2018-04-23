/******************************************************************************/
/*!
@file   WindowDebug.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using System;
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System. Collections.Generic;
using System.Security.Policy;

namespace Stratus
{
  namespace UI
  {
    public class WindowInspector : MonoBehaviour
    {
      /// <summary>
      /// The current window being inspected
      /// </summary>
      [ReadOnly] public Window Target;

      /// <summary>
      /// The dropdown menu used to select what window to debug
      /// </summary>
      private Dropdown Dropdown;

      /// <summary>
      /// The text describing the current link
      /// </summary>
      public Text CurrentLinkText;

      /// <summary>
      /// The windows currently available to inspect
      /// </summary>
      private Window[] AvailableWindows;
      
      /// <summary>
      /// Configures the dropdown list of available windows 
      /// </summary>
      void Start()
      {
        Dropdown = GetComponentInChildren<Dropdown>();
        this.Populate();
        this.ChangeWindow(0);
      }

      /// <summary>
      /// Changes the current window
      /// </summary>
      /// <param name="index"></param>
      public void ChangeWindow(int index)
      {
        //Trace.Script("Index = " + index, this);
        Dropdown.RefreshShownValue();
        //Dropdown.value = index;
        // Disconnect from all previousi events
        this.Disconnect();
        // Set the current window
        this.Target = AvailableWindows[index];
        // Subscribe to its events
        this.Target.gameObject.Connect<Link.SelectEvent>(this.OnLinkSelectEvent);
        // Show its current link
        ShowCurrentLink();
        //Trace.Script("Selected window is now " + this.Target.name, this);
      }

      void OnLinkSelectEvent(Link.SelectEvent e)
      {
        ShowCurrentLink();
      }

      /// <summary>
      /// Shows the currently selected link
      /// </summary>
      void ShowCurrentLink()
      {
        if (Target.CurrentLink != null)
          this.CurrentLinkText.text = this.Target.CurrentLink.name;
      }

      /// <summary>
      /// Populates the dropdown list
      /// </summary>
      void Populate()
      {
        AvailableWindows = (Window[])FindObjectsOfType(typeof(Window));
        if (AvailableWindows.Length == 0) throw new SystemException("No valid Window components on the scene!");

        var names = new List<string>();
        foreach (var window in AvailableWindows)
          names.Add(window.name);

        // Configure the dropdown list
        Dropdown.ClearOptions();
        Dropdown.AddOptions(names);
        Dropdown.RefreshShownValue();
      }

      /// <summary>
      /// Opens the target window
      /// </summary>
      public void Open()
      {
        if (!this.Target)
          return;

        Target.RequestOpen();
        ShowCurrentLink();
      }

      /// <summary>
      /// Closes the target window
      /// </summary>
      public void Close()
      {
        if (!this.Target)
          return;

        //this.CurrentLinkText.text.Clear();
        this.CurrentLinkText.text = "";
        Target.RequestClose();
      }

      


    } 
  }
}
