/******************************************************************************/
/*!
@file   Button.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using Stratus.UI;
using System;
using UnityEngine.UI;

namespace Stratus 
{
  namespace UI
  {
    /// <summary>
    /// Represents a button that can be pressed
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour
    {
      [Serializable]
      public class ButtonStyle
      {
        public Color Default = Color.white;
        public Color Pressed = Color.red;
        public Color Highlight = Color.yellow;
        public Color Disabled = Color.gray;
        public float Duration = 0.25f;
      }

      IEnumerator Routine;

      [HideInInspector] public Image Image;
      public ButtonStyle Style = new ButtonStyle();
      [Tooltip("What sound to play when the button is pressed")]
      public AudioClip Sound = new AudioClip();
      /// <summary>
      /// What sprite to use for this button
      /// </summary>
      public Sprite Sprite
      {
        set { GetComponent<Image>().sprite = value; }
        get { return GetComponent<Image>().sprite; }
      }

      public void Show(bool show)
      {
        if (show)
        {
          Image.CrossFadeAlpha(1.0f, 0.2f, true);
        }
        else
        {
          Image.CrossFadeAlpha(0.0f, 0.2f, true);
        }
      }

      void Start()
      {
        Image = GetComponent<Image>();
        Routine = Transition();
      }

      public void Enable()
      {
        Image.CrossFadeColor(Style.Default, Style.Duration, true, true);
      }

      public void Disable()
      {
        Image.CrossFadeColor(Style.Disabled, Style.Duration, true, true);
      }

      public void Highlight()
      {
        Image.CrossFadeColor(Style.Highlight, Style.Duration, true, true);
      }

      /// <summary>
      /// Presses the button!
      /// </summary>
      public void Press()
      {
        //Trace.Script("Pressed!", this);          
        SoundEffects.PlayEffect(this.Sound);
        Image.CrossFadeColor(Style.Pressed, Style.Duration, true, true);
        StopCoroutine(Routine);
        StartCoroutine(Routine);        
      }

      IEnumerator Transition()
      {
        while (true)
        {
          yield return new WaitForSeconds(Style.Duration);
          Image.CrossFadeColor(Style.Default, Style.Duration, true, true);
          yield return null;
          //yield return new WaitForSeconds(Style.Duration);
        }
      }
      
      /// <summary>
      /// Destroys this button within the specified amount of time.
      /// </summary>
      /// <param name="t">How long before this button is destroyed.</param>
      public void Destroy(float t)
      {
        var seq = Actions.Sequence(this);
        Actions.Delay(seq, t + this.Style.Duration);
        Actions.Call(seq, this.gameObject.Destroy);
      }








    } 
  }
}
