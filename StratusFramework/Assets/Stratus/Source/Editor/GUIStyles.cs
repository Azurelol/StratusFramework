/******************************************************************************/
/*!
@file   GUIStyles.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; 
#endif

namespace Stratus
{
  namespace Utilities
  {
    [CreateAssetMenu(fileName = "GUIStyle", menuName = "Stratus/GUI/Styles")]
    public class GUIStyles : ScriptableObject
    {
      /// <summary>
      /// The name of this style
      /// </summary>
      public string Name;
      /// <summary>
      /// The style
      /// </summary>
      public GUIStyle Style;
      /// <summary>
      /// What
      /// </summary>            
      public GUISkin Skin;

      /// <summary>
      /// Looks for a given style by name
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      public static GUIStyle GetStyle(string name)
      {
        // Look for the style in the path
        return null;
      }
    }


    public static class GUITools
    {
      public enum Style
      {
        Title
      }

      public static GUIStyle Get(Style style)
      {
        #if UNITY_EDITOR
        switch (style)
        {
          case Style.Title:
            return EditorStyles.largeLabel;
        }
        #endif

        return null;
      }

    }
  }
}