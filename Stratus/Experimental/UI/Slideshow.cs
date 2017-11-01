/******************************************************************************/
/*!
@file   Slideshow.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// Presents a series of Graphical objects in sequence, using events
  /// and triggers to fade them in and out.
  /// </summary>
  public class Slideshow : MonoBehaviour
  {
    public enum DurationType { Each, Total }
    public KeyCode SkipKey = KeyCode.Escape;
    public float Duration;
    [Tooltip("Whether this is the total duration or for each slide")]
    public DurationType Type = DurationType.Total;
    public Ease TransitionEase = Ease.Linear;
    public bool TriggerOnEnd = true;

    Queue<Transform> Slides = new Queue<Transform>();

    /**************************************************************************/
    /*!
    @brief  Initializes the SplashScreen.
    */
    /**************************************************************************/
    void Start()
    {
      this.AddSlides();
      this.StartSlideshow();
    }

    void AddSlides()
    {
      // Add all children
      foreach (var child in this.gameObject.Children())
      {
        Slides.Enqueue(child.transform);
        var graphical = child.gameObject.GetComponent<Graphic>();
        graphical.color = Color.clear;
      }
    }

    void StartSlideshow()
    {
      var seq = Actions.Sequence(this);

      // We will divide the duration evenly for the number of splashes
      float durationForEach;
      if (this.Type == DurationType.Total)
        durationForEach = this.Duration / Slides.Count;
      else
        durationForEach = this.Duration;

      foreach (var slide in Slides)
      {
        slide.gameObject.SetActive(true);
        var graphical = slide.gameObject.GetComponent<Graphic>();
        Actions.Property(seq, () => graphical.color, Color.white, durationForEach, TransitionEase);
        Actions.Call(seq, this.AdvanceSlide);
        Actions.Property(seq, () => graphical.color, Color.clear, durationForEach, TransitionEase);
      }

      // Now exit
      Actions.Call(seq, this.End);
    }

    /// <summary>
    /// Advances to the next slide.
    /// </summary>
    void AdvanceSlide()
    {
      var nextSlide = Slides.Dequeue();
      //Trace.Script("Changing to '" + nextSlide.gameObject.name + "'");
      nextSlide.gameObject.SetActive(true);      

    }


    void Update()
    {
      if (Input.GetKeyDown(SkipKey))
      {
        this.End();
      }
    }

    void End()
    {
      if (TriggerOnEnd)
      {
        this.gameObject.Dispatch<Trigger.TriggerEvent>(new Trigger.TriggerEvent());
      }
    }

  }
}