/******************************************************************************/
/*!
@file   GUIStyles.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace Editors
  {
    public static class Styles
    {
      public static GUIStyle EditorLine { get; private set; }
      public static GUIStyle Tintable { get; private set; }
    
      static Styles()
      {
        EditorLine = new GUIStyle(GUI.skin.box);
        EditorLine.border.top = EditorLine.border.bottom =
        EditorLine.border.left = EditorLine.border.right = 1;
        EditorLine.margin.top = EditorLine.margin.bottom =
        EditorLine.margin.left = EditorLine.margin.right = 1;
        EditorLine.padding.top = EditorLine.padding.bottom =
        EditorLine.padding.left = EditorLine.padding.right = 1;

        Tintable = new GUIStyle();
        Tintable.normal.background = EditorGUIUtility.whiteTexture;
        Tintable.stretchWidth = Tintable.stretchHeight = true;        
      }
      
      public static void DrawBackgroundColor(Rect position, Color color)
      {
        if (UnityEngine.Event.current.type == EventType.Repaint)
        {
          var prevColor = GUI.color;
          GUI.color = color;
          Tintable.Draw(position, false, false, false, false);
          GUI.color = prevColor;
        }
      }
      
    }



    
  }
}