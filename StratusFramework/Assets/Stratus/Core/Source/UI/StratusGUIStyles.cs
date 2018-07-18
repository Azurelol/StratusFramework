using UnityEngine;
using System.Collections.Generic;
using System;

namespace Stratus
{
  /// <summary>
  /// Custom styles for the Stratus framework
  /// </summary>
  public static partial class StratusGUIStyles
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public enum Border
    {
      Full,
      Left,
      Right
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The default skin used by the Stratus framework
    /// </summary>
    public static GUISkin skin { get; private set; }
    /// <summary>
    /// Common style for labels
    /// </summary>
    public static GUIStyle label { get; private set; }
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
    public static Color backgroundColor => Colors.azure;
    /// <summary>
    /// The default color for connections
    /// </summary>
    public static Color connectedColor => Colors.jade;
    /// <summary>
    /// The default color for connections
    /// </summary>
    public static Color disconnectedColor => Colors.saffron;
    /// <summary>
    /// The default color for connections
    /// </summary>
    public static Color selectedColor => Colors.royalBlue;
    /// <summary>
    /// A map of all custom textures used by the framework
    /// </summary>
    public static Dictionary<string, Texture2D> textures { get; set; } = new Dictionary<string, Texture2D>();
    /// <summary>
    /// A map of all custom textures used by the framework
    /// </summary>
    public static Dictionary<string, Font> fonts { get; set; } = new Dictionary<string, Font>();
    /// <summary>
    /// A map of all colored backgrounds generated 
    /// </summary>
    private static Dictionary<Color, GUIStyle> coloredBackgrounds = new Dictionary<Color, GUIStyle>();
    /// <summary>
    /// A map of all colored backgrounds generated 
    /// </summary>
    private static Dictionary<Color, Texture2D> coloredTextures = new Dictionary<Color, Texture2D>();

    // Styles    
    public static GUIStyle box => skin.box;
    public static GUIStyle button => skin.button;
    public static GUIStyle miniButton => skin.FindStyle("Mini Button");
    public static GUIStyle outlineBox => skin.FindStyle("Outline Box");
    public static GUIStyle circleButton => skin.FindStyle("Circle Button");
    public static GUIStyle tickButton => skin.FindStyle("Tick Button");
    public static GUIStyle header { get; set; }
    public static GUIStyle headerWhite => skin.FindStyle("Header White");
    public static GUIStyle headerBlack => skin.FindStyle("Header");
    public static GUIStyle miniText => skin.FindStyle("Mini Text");
    public static GUIStyle background => skin.FindStyle("Background");
    public static GUIStyle whiteBorder => skin.FindStyle("White Border");
    public static GUIStyle whiteRightBorder => skin.FindStyle("White Right Border");
    public static GUIStyle whiteLeftBorder => skin.FindStyle("White Left Border");
    public static GUIStyle listViewLabel { get; private set; }
    public static GUIStyle listViewToggle { get; private set; }
    public static GUIStyle listViewTextField { get; private set; }
    public static GUIStyle popup { get; private set; }
    public static Font defaultFont { get; private set; }
    public static Font boldFont { get; private set; }
    public static Font lightFont { get; private set; }
    //public static GUIStyle currentHeader

    public static bool isProSkin { get; set; }

    // Textures
    public static Texture2D optionsTexture => textures["cog"];
    public static Texture2D addTexture => textures["plus"];
    public static Texture2D messageTexture => textures["talk"];
    public static Texture2D trashTexture => textures["trash-can"];
    public static Texture2D validateTexture => textures["magnifying-glass"];
    // Layouts

    public static GUILayoutOption[] smallLayout { get; private set; }
    public static GUILayoutOption[] singleLineLayout { get; private set; }



    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    static StratusGUIStyles()
    {
      // Load custom assets
      LoadGUIAssets(); 
      // Set colors
      SetColors();
      // Store default options
      CacheDefaultOptions();


    }
    
    private static void LoadGUIAssets()
    {
      // Load the default skin
      skin = Resources.Load<GUISkin>("Stratus Skin");

      // Load gui assets
      Texture2D[] assets = Resources.LoadAll<Texture2D>(StratusCore.guiFolder);
      foreach(var asset in assets)
      {
        textures.Add(asset.name, asset);
      }

      // Load fonts
      Font[] fontAssets = Resources.LoadAll<Font>(StratusCore.fontFolder);
      foreach(var font in fontAssets)
      {
        fonts.Add(font.name, font);
      }
      defaultFont = fonts["OpenSans-Regular"];
      boldFont = fonts["OpenSans-Bold"];
      lightFont = fonts["OpenSans-Light"];

      // Set defaults
      label = skin.label;
      listViewTextField = new GUIStyle(skin.textField);
      listViewTextField.stretchHeight = false;
      listViewLabel = skin.FindStyle("List Label");
      listViewLabel.wordWrap = true;
      listViewLabel.stretchHeight = false;
      listViewToggle = new GUIStyle(skin.toggle);
      listViewLabel.margin = listViewTextField.margin;
      listViewLabel.border = listViewTextField.border;
      listViewLabel.padding = listViewTextField.padding;
      

#if UNITY_EDITOR
      popup = new GUIStyle(UnityEditor.EditorStyles.popup);
      popup.richText = true;
      popup.font = boldFont;
      popup.fontSize = 10;      
      #endif
    }

    private static void CacheDefaultOptions()
    {
      //editorLine = new GUIStyle(GUI.skin.box);
      //editorLine.border.top = editorLine.border.bottom =
      //editorLine.border.left = editorLine.border.right = 1;
      //editorLine.margin.top = editorLine.margin.bottom =
      //editorLine.margin.left = editorLine.margin.right = 1;
      //editorLine.padding.top = editorLine.padding.bottom =
      //editorLine.padding.left = editorLine.padding.right = 1;

      tintable = new GUIStyle();
      tintable.normal.background = Texture2D.whiteTexture;
      tintable.stretchWidth = tintable.stretchHeight = true;

      smallLayout = new GUILayoutOption[] { GUILayout.Width(25f), GUILayout.Height(25f) };

#if UNITY_EDITOR
      //UnityEditor.EditorStyles.helpBox.richText = true;
      isProSkin = UnityEditor.EditorGUIUtility.isProSkin;
      header = isProSkin ? headerWhite : headerBlack;
      singleLineLayout = new GUILayoutOption[] { GUILayout.Width(UnityEditor.EditorGUIUtility.singleLineHeight), GUILayout.Height(UnityEditor.EditorGUIUtility.singleLineHeight)};
#endif
    }

    /// <summary>
    /// Set common colors used by the framework
    /// </summary>
    private static void SetColors()
    {
      Colors.azure = new Color32(0, 191, 255, 255);
      Colors.sanMarino = new Color32(66, 108, 179, 255);
      Colors.royalBlue = new Color32(65, 131,215, 255);
      Colors.jacksonsPurple = new Color32(31, 58, 147, 255);
      Colors.aquaIsland = new Color32(162, 222, 208, 255);
      Colors.jade = new Color32(0, 177, 106, 255);
      Colors.cinnabar= new Color32(240, 52, 52, 255);
      Colors.valencia = new Color32(214, 69, 65, 255);
      Colors.chestnutRose = new Color32(210, 77, 87, 255);
      Colors.saffron = new Color32(244, 208, 63, 255);
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
    /// Draws an outline around the given rect
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public static void DrawOutline(Rect position, Color color, Border border = Border.Full)
    {
      if (UnityEngine.Event.current.type == EventType.Repaint)
      {
        var prevColor = GUI.color;
        GUI.color = color;
        switch (border)
        {
          case Border.Full:
            whiteBorder.Draw(position, false, false, false, false);
            break;

          case Border.Left:
            whiteLeftBorder.Draw(position, false, false, false, false);
            break;

          case Border.Right:
            whiteRightBorder.Draw(position, false, false, false, false);
            break;
        }
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
    public static GUIStyle ColoredBackground(Color color)
    {
      if (!coloredBackgrounds.ContainsKey(color))
      {
        var cb = new GUIStyle();
        cb.normal.background = MakeTexture(button.normal.background, color);
        coloredBackgrounds.Add(color, cb);
      }

      return coloredBackgrounds[color];      
    }

    public static Texture2D GetColorTexture(Color color)
    {
      if (!coloredTextures.ContainsKey(color))
      {
        coloredTextures.Add(color, MakeTexture(Texture2D.whiteTexture, color));
      }

      return coloredTextures[color];
    }

    

  }

}