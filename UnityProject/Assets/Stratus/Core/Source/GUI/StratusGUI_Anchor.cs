/******************************************************************************/
/*!
@file   Overlay_Utility.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System;

namespace Stratus
{
  public partial class StratusGUI
  {
    //------------------------------------------------------------------------/
    // Enumerations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Where should the window be anchored to
    /// </summary>
    public enum Anchor
    {
      Center,
      Top,
      Bottom,
      Left,
      Right,
      TopLeft,
      TopRight,
      BottomLeft,
      BottomRight
    }

    /// <summary>
    /// Whether to use the dimensions as an absolute value in pixels, or a relative percentage based on screen size
    /// </summary>
    public enum Dimensions
    {
      /// <summary>
      /// The provided size is to be taken in respect to the size of the containing rect (the screen)
      /// </summary>
      Relative,
      /// <summary>
      /// The provided size is to be used directly in pixels
      /// </summary>
      Absolute
    }

    /// <summary>
    /// Finds the dimensions relative to screen space, from a given percentage (0 to 1).
    /// So for example if you wanted to cover 10% of the screen's width and 20% of its height,
    /// you would pass in (0.1f, 0,2f)
    /// </summary>
    /// <param name="widthRatio">The relative width of the screen as a percentage (from 0 to 1)</param>
    /// <param name="heightRatio">The relative height of the screen as a percentage (from 0 to 1)</param>
    /// <returns></returns>
    public static Vector2 FindRelativeDimensions(Vector2 sizeRatio, Vector2 screenSize)
    {
      if (sizeRatio.x < 0f || sizeRatio .x > 1f || sizeRatio .y < 0f || sizeRatio.y > 1f)
        throw new ArgumentOutOfRangeException("Expected a value between 0 and 1!");
      return new Vector2(screenSize.x * sizeRatio.x, screenSize.y * sizeRatio.y);
    }

    /// <summary>
    /// Computes a proper rect from a given anchored position along with the width and height
    /// </summary>
    /// <param name="anchor">The relative position of the rect in screen space</param>
    /// <param name="size">The dimensions of the rect, relative to the given screen size</param>
    /// <returns></returns>
    public static Rect CalculateAnchoredPositionOnScreen(Anchor anchor, Vector2 size)
    {
      return CalculateAnchoredPositionOnScreen(anchor, size, screenSize);
    }

    /// <summary>
    /// Computes a proper rect from a given anchored position along with the width and height
    /// </summary>
    /// <param name="anchor">The relative position of the rect in screen space</param>
    /// <param name="size">The dimensions of the rect, relative to the given screen size</param>
    /// <param name="screenSize">The dimensions of the screen the rect will be in</param>
    /// <returns></returns>
    public static Rect CalculateAnchoredPositionOnScreen(Anchor anchor, Vector2 size, Vector2 screenSize)
    {
      var rect = new Rect();

      //if (mode == Dimensions.Relative)
      //  size = FindRelativeDimensions(size, screenSize);

      float width = size.x;
      float height = size.y;

      float screenWidth = screenSize.x;
      float screenHeight = screenSize.y;

      const float padding = 8f;
      // This is stupid. I couldn't figure out why it won't position properly otherwise if anchored to the bottom
      const float bottomMultiplier = 3f;

      // Find the x and y positions depending on the anchor
      float x = 0f;
      float y = 0f;

      switch (anchor)
      {
        case Anchor.Center:          
          x = screenWidth / 2 - (width / 2);
          y = screenHeight / 2 - (height / 2);
          break;
        case Anchor.Top:
          x = screenWidth / 2 - (width / 2);
          y = padding;
          break;
        case Anchor.TopLeft:
          x = padding;
          y = padding;
          break;
        case Anchor.TopRight:
          x = screenWidth - width - padding;
          y = padding;
          break;
        case Anchor.Left:
          x = padding;
          y = screenHeight / 2 - ( (height / 2 ) - padding);
          break;
        case Anchor.Right:
          x = screenWidth - width - padding;
          y = screenHeight / 2 - ((height / 2) - padding);
          break;
        case Anchor.Bottom:
          x = screenWidth / 2 - (width / 2);
          y = screenHeight - height - (padding * bottomMultiplier);
          break;
        case Anchor.BottomLeft:
          x = padding;
          y = screenHeight - height - (padding * bottomMultiplier);
          break;
        case Anchor.BottomRight:
          x = screenWidth - width - padding;
          y = screenHeight - height - (padding * bottomMultiplier);
          break;
      }

      // Set the values
      rect.x = x;
      rect.y = y;
      rect.width = width;
      rect.height = height;

      return rect;
    }

    /// <summary>
    /// Makes a texture
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="textureColor"></param>
    /// <param name="border"></param>
    /// <param name="bordercolor"></param>
    /// <returns></returns>
    private static Texture2D MakeTexture(int width, int height, Color textureColor, RectOffset border, Color bordercolor)
    {
      int widthInner = width;
      width += border.left;
      width += border.right;

      Color[] pix = new Color[width * (height + border.top + border.bottom)];



      for (int i = 0; i < pix.Length; i++)
      {
        if (i < (border.bottom * width))
          pix[i] = bordercolor;
        else if (i >= ((border.bottom * width) + (height * width)))  //Border Top
          pix[i] = bordercolor;
        else
        { //Center of Texture

          if ((i % width) < border.left) // Border left
            pix[i] = bordercolor;
          else if ((i % width) >= (border.left + widthInner)) //Border right
            pix[i] = bordercolor;
          else
            pix[i] = textureColor;    //Color texture
        }
      }

      Texture2D result = new Texture2D(width, height + border.top + border.bottom);
      result.SetPixels(pix);
      result.Apply();


      return result;
    }

    private static Texture2D MakeTexture(int width, int height, Color col)
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