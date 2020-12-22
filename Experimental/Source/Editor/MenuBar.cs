using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Stratus
{
  namespace Editor
  {
    /// <summary>
    /// A configurable menu bar to be used in an editor
    /// </summary>
    public class MenuBar
    {
      public class MenuPair
      {
        public string Title;
        public GenericMenu Menu;
        public MenuPair(string title, GenericMenu menu)
        {
          this.Title = title;
          this.Menu = menu;
        }
      }

      public class StyleSettings
      {
        public GUIStyle Menu;
        public GUIStyle Dropdown;

        public StyleSettings(GUIStyle menu, GUIStyle dropdown)
        {
          this.Menu = menu;
          this.Dropdown = dropdown;
        }
      }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      private List<MenuPair> Menus = new List<MenuPair>();
      public StyleSettings Styles { set; get; }

      //------------------------------------------------------------------------/
      // Constructors
      //------------------------------------------------------------------------/
      public MenuBar()
      {
      }

      public MenuBar(StyleSettings styles)
      {        
        this.Styles = styles;
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public GenericMenu Add(string title)
      {
        var menu = new GenericMenu();
        Menus.Add(new MenuPair(title, menu));
        return menu;
      }

      public void Draw(Rect position)
      {
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        var currentEvent = UnityEngine.Event.current;
        foreach (var menu in Menus)
        {
          if (GUILayout.Button(menu.Title, EditorStyles.label, GUILayout.ExpandWidth(false)))
          {
            menu.Menu.ShowAsContext();
            currentEvent.Use();
          }
        }

        GUILayout.EndHorizontal();
        //GUILayout.Box(GUIContent.none, Editors.Styles.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));

      }


    }








  }
}