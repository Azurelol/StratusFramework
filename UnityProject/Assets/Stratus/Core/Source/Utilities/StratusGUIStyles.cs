/******************************************************************************/
/*!
@file   GUIStyles.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
//using UnityEditor;

namespace Stratus
{
  /// <summary>
  /// Custom styles for the Stratus framework
  /// </summary>
  public static class StratusGUIStyles
  {
    /// <summary>
    /// The default skin used by the Stratus framework
    /// </summary>
    public static GUISkin skin { get; private set; }
    /// <summary>
    /// A thin line for used within an editor window
    /// </summary>
    public static GUIStyle editorLine { get; private set; }
    /// <summary>
    /// A tintable background
    /// </summary>
    public static GUIStyle tintable { get; private set; }
    /// <summary>
    /// The main background color used by UI elmeents
    /// </summary>
    public static Color backgroundColor => azureColor;
    /// <summary>
    /// The default color used for Stratus
    /// </summary>
    public static Color azureColor { get; set; }
    public static GUIStyle box => skin.box;
    public static GUIStyle button => skin.button;
    public static GUIStyle blueButton => skin.FindStyle("Blue Button");
    public static GUIStyle greenButton => skin.FindStyle("Green Button");
    public static GUIStyle filledBox => skin.FindStyle("Filled Box");
    public static GUIStyle outlineBox => skin.FindStyle("Outline Box");
    public static GUIStyle greyCircleButton => skin.FindStyle("Grey Circle Button");
    public static GUIStyle blueCircleButton => skin.FindStyle("Blue Circle Button");
    public static GUIStyle blueBoxTickButton => skin.FindStyle("Blue Box Tick Button");
    public static GUIStyle header => skin.FindStyle("Header");
    public static GUIStyle miniText => skin.FindStyle("Mini Text");
    public static GUIStyle backgroundLight => skin.FindStyle("Light Background");
    public static GUIStyle background => skin.FindStyle("Background");

    static StratusGUIStyles()
    {
      // Load the default skin
      skin = Resources.Load<GUISkin>("Stratus Skin"); 

      azureColor = new Color(0, 191, 255);
      editorLine = new GUIStyle(GUI.skin.box);
      editorLine.border.top = editorLine.border.bottom =
      editorLine.border.left = editorLine.border.right = 1;
      editorLine.margin.top = editorLine.margin.bottom =
      editorLine.margin.left = editorLine.margin.right = 1;
      editorLine.padding.top = editorLine.padding.bottom =
      editorLine.padding.left = editorLine.padding.right = 1;

      #if UNITY_EDITOR
      tintable = new GUIStyle();
      tintable.normal.background = UnityEditor.EditorGUIUtility.whiteTexture;
      tintable.stretchWidth = tintable.stretchHeight = true;
      #endif

    }

    /// <summary>
    /// Draws the selected background color inside the given rect
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public static void DrawBackgroundColor(Rect position, Color color)
    {
      if (UnityEngine.Event.current.type == EventType.Repaint)
      {
        var prevColor = GUI.color;
        GUI.color = color;
        tintable.Draw(position, false, false, false, false);
        GUI.color = prevColor;
      }
    }

    /// <summary>
    /// Generates a 2D texture given another texture to use for dimensions and the color
    /// </summary>
    /// <param name="other"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Texture2D MakeTexture(Texture2D other, Color color)
    {
      return MakeTexture(other.width, other.height, color);
    }

    /// <summary>
    /// Generates a 2D texture
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public static Texture2D MakeTexture(int width, int height, Color col)
    {
      Color[] pix = new Color[width * height];

      for (int i = 0; i < pix.Length; i++)
        pix[i] = col;

      Texture2D result = new Texture2D(width, height);
      result.SetPixels(pix);
      result.Apply();

      return result;
    }

  }

}