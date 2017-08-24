/******************************************************************************/
/*!
@file   Overlay_Element.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  public partial class Overlay
  {
    /// <summary>
    /// An UI element of the overlay
    /// </summary>         
    public abstract class Element
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/        
      /// <summary>
      /// Whether this element is currently enabled
      /// </summary>
      public bool enabled = true;      

      /// <summary>
      /// The name of this element
      /// </summary>
      public string name;

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="name"></param>
      public Element(string name)
      {
        this.name = name;
      }
        
      /// <summary>
      /// Draws this element
      /// </summary>
      public void Draw()
      {
        if (!enabled)
          return;

        this.OnDraw();
      }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      /// <summary>
      /// Draws this element
      /// </summary>
      protected abstract void OnDraw();
    }
  }

}
