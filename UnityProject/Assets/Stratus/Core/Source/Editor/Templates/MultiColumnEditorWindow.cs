using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace Editor
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
      private bool initialized { get; set; }

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
      private MenuBar menu;
      private GUISplitter columns;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnMultiColumnEditorEnable(MenuBar menu, GUISplitter columns);

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      private void OnEnable()
      {
        // Add the menu items
        menu = new MenuBar();
        menu.Styles = new MenuBar.StyleSettings(EditorStyles.toolbar, EditorStyles.toolbarDropDown);
        // Add columns, then validate (to make sure all their widths add up to 1)
        columns = new GUISplitter(this, GUISplitter.OrientationType.Horizontal);

        
      }

      private void OnGUI()
      {
        if (!initialized)
        {
          OnMultiColumnEditorEnable(menu, columns);
          initialized = true;
        }

        menu.Draw(MenuPosition);
        columns.Draw(BodyPosition);
      }         

    } 
  }
}