using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Stratus
{
  [DisallowMultipleComponent]
  public class ObjectNote : MonoBehaviour
  {
    /// <summary>
    /// When to draw this note
    /// </summary>
    public enum DrawMode
    {
      Always,
      Selected,
      Unselected,
      Never
    }

    [TextArea]
    public string text = "Note";
    [Header("Settings")]
    public DrawMode drawMode = DrawMode.Always;
    public Color color = Color.yellow;
    public Vector3 offset = new Vector3();
    [Header("Text")]
    public FontStyle fontStyle = FontStyle.Normal;
    public int fontSize = 10;
    public TextAnchor alignment = TextAnchor.UpperLeft;
    
    [HideInInspector]
    public GUIStyle style;
    private static RectOffset padding; // = new RectOffset(6, 0, 5, 0);
    private int fixedHeight = 0;

    public bool hasStyle { get; set; }

    private void OnEnable()
    {
      Hide();
    }

    private void OnValidate()
    {
      hasStyle = false;
      //ConstructStyle();
    }

    void Hide()
    {
      hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy;
    }
      

    public void ConstructStyle()
    {
      //  Texture2D whiteTex = new Texture2D(1, 1);
      // whiteTex.SetPixel(0, 0, Color.white);
      // Style.normal.background = whiteTex;
     if (padding == null)
        padding = new RectOffset(6, 0, 5, 0);

      style = new GUIStyle("Box");
      style.normal.background = Texture2D.whiteTexture;
      style.fontSize = fontSize;
      style.fontStyle = fontStyle;
      style.fixedHeight = fixedHeight;
      style.padding = padding;
      style.wordWrap = true;
      style.alignment = alignment;
      hasStyle = true;
    }
    
  }


}


