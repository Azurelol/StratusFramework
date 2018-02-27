using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace Editors
  {
    /// <summary>
    /// An editor supporting multiple columns as well as a menu bar
    /// </summary>
    public abstract class MultiColumnEditorWindow : EditorWindow
    {
      //------------------------------------------------------------------------/
      // Classes
      //------------------------------------------------------------------------/
      public class Styles
      {
      }

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// Whether this editor has bee initialized (it is initialized during the very first
      /// update)
      /// </summary>
      private bool Initialized { get; set; }

      /// <summary>
      /// The height of the menu bar this window uses (used as an offset for the columns)
      /// </summary>
      float MenuHeight { get { return EditorStyles.toolbarDropDown.fixedHeight - 1f; } }
      
      /// <summary>
      /// The position of the body (the columns) for this editor
      /// </summary>
      private Rect BodyPosition
      {
        get
        {
          return new Rect(0f, MenuHeight, this.position.width, this.position.height - MenuHeight); 
        }
      }

      /// <summary>
      /// The position for the menu bar for this editor
      /// </summary>
      private Rect MenuPosition
      {
        get
        {
          return new Rect(0f, 0f, this.position.width, MenuHeight);
        }
      }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      private MenuBar Menus;
      private GUISplitter Columns;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnInitialize();
      protected abstract void SetStyles(MenuBar.StyleSettings menuStyle);
      protected abstract void AddColumns(GUISplitter columns);
      protected abstract void AddMenus(MenuBar menu);

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      private void OnEnable()
      {
        // Add the menu items
        Menus = new MenuBar();
        AddMenus(Menus);
        // Add columns, then validate (to make sure all their widths add up to 1)
        Columns = new GUISplitter(this, GUISplitter.OrientationType.Horizontal);
        AddColumns(Columns);
        // Set the window as not initialized yet
        Initialized = false;
      }
      private void OnGUI()
      {
        if (!Initialized)
          Initialize();

        Menus.Draw(MenuPosition);
        Columns.Draw(BodyPosition);
      }      

      void Initialize()
      {
        // Set all styles
        Menus.Styles = new MenuBar.StyleSettings(EditorStyles.toolbar, EditorStyles.toolbarDropDown);
        SetStyles(Menus.Styles);

        // Initialize the subclass
        OnInitialize();

        Initialized = true;
      }            

    } 
  }
}