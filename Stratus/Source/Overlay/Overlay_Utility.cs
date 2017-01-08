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
  public partial class Overlay
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
    /// Finds the dimensions relative to screen space, from a given percentage (0 to 1).
    /// So for example if you wanted to cover 10% of the screen's width and 20% of its height,
    /// you would pass in (0.1f, 0,2f)
    /// </summary>
    /// <param name="widthRatio">The relative width of the screen as a percentage (from 0 to 1)</param>
    /// <param name="heightRatio">The relative height of the screen as a percentage (from 0 to 1)</param>
    /// <returns></returns>
    public static Vector2 FindRelativeDimensions(float widthRatio, float heightRatio)
    {
      if (widthRatio < 0f || widthRatio > 1f || heightRatio < 0f || heightRatio > 1f)
        throw new ArgumentOutOfRangeException("Expected a value between 0 and 1!");
      return new Vector2(Screen.width * widthRatio, Screen.height * heightRatio);
    }


    /// <summary>
    /// Computes a proper rect from a given anchored position along with the width and height
    /// </summary>
    /// <param name="anchor">The relative position of the rect in screen space</param>
    /// <param name="width">The width the rect should have</param>
    /// <param name="height">The height the rect should have</param>
    /// <returns></returns>
    public static Rect FindAnchoredPosition(Anchor anchor, float width, float height)
    {
      var rect = new Rect();

      // Find the x and y positions depending on the anchor
      float x = 0f, y = 0f;
      switch (anchor)
      {
        case Anchor.Center:
          x = Screen.width / 2 - (width / 2);
          y = Screen.height / 2 - (height / 2);
          break;
        case Anchor.TopLeft:
          x = 0f;
          y = 0f;
          break;
        case Anchor.TopRight:
          x = Screen.width - width;
          y = 0f;
          break;
        case Anchor.BottomLeft:
          x = 0f;
          y = Screen.height - height;
          break;
        case Anchor.BottomRight:
          x = Screen.width - width;
          y = Screen.height - height;
          break;
        case Anchor.Left:
          x = 0f;
          y = Screen.height / 2 - (height / 2);
          break;
        case Anchor.Right:
          x = Screen.width - width;
          y = 0f;
          break;
        case Anchor.Top:
          x = Screen.width / 2 - (width / 2);
          y = 0f;
          break;
        case Anchor.Bottom:
          x = Screen.width / 2 - (width / 2);
          y = Screen.height - height;
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