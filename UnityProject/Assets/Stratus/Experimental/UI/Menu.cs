/******************************************************************************/
/*!
@file   Menu.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Base class for all menus
    /// </summary>
    public abstract class Menu : Window
    {
      public Window RootWindow;
      public bool CloseOnCancel = true;

      /**************************************************************************/
      /*!
      @brief  Initializes the Script.
      */
      /**************************************************************************/
      protected override void OnWindowInitialize()
      {
        //Trace.Script("Subsribing to events", this);
        // Called on the menu subclass
        InitializeMenu();
      }

      protected abstract void InitializeMenu();

      protected override void OnLinkOpened()
      {
        DisplayRoot(false);
      }

      protected override void OnLinkClosed()
      {
        DisplayRoot(true);
      }

      /**************************************************************************/
      /*!
      @brief Opens the menu.
      */
      /**************************************************************************/
      protected override void OnWindowOpen()
      {
        this.Fit();
        // Open the root window and display it
        if (RootWindow) RootWindow.RequestOpen();
        DisplayRoot(true);
        // Announce the gamestate change
        OnMenuOpen();
      }

      protected override void OnWindowClose()
      {
        if (RootWindow) RootWindow.RequestClose();
        OnMenuClose();
      }

      protected override void OnInterfaceCancel()
      {
        if (CloseOnCancel)
          this.Close();
      }

      protected abstract void OnMenuOpen();
      protected abstract void OnMenuClose();

      /**************************************************************************/
      /*!
      @brief Displays the root window.
      */
      /**************************************************************************/
      void DisplayRoot(bool display)
      {
        if (!RootWindow)
          return;

        if (display)
        {
          RootWindow.Toggle(true);
        }
        else
        {
          RootWindow.Toggle(false);
        }
      }

    }
  } 
}