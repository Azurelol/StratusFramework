/******************************************************************************/
/*!
@file   WindowElement.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using Stratus.Utilities;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// 
    /// </summary>
    public class WindowElement : MonoBehaviour
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public bool Hidden;

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      void Awake()
      {
        if (Hidden)
        {
          //Trace.Script("Hiding", this);
          Graphical.Fade(this, 0.0f, 0.0f);
        }
      }

      public void Show(float duration)
      {
        //Trace.Script("Now showing", this);
        Graphical.Fade(this, 1.0f, 0.0f);
      }

      public void Hide(float duration)
      {
        Graphical.Fade(this, 0.0f, duration);
      }

    }

  }
}