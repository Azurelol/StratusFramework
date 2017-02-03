/******************************************************************************/
/*!
@file   SoundEffects.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Plays sound effects used by the UI.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffects : StratusSingleton<SoundEffects>
    {
      public class PlayEvent : Stratus.Event { public AudioClip Clip; }

      private AudioSource Player;
      protected override string Name { get { return "SoundEffects"; } }
      protected override void OnAwake()
      {
        DontDestroyOnLoad(this);
        Player = this.gameObject.GetComponent<AudioSource>();
        Player.loop = false;
        Scene.Connect<PlayEvent>(this.OnPlayEvent);
      }

      void OnPlayEvent(PlayEvent e)
      {
        this.Play(e.Clip);
      }

      void Play(AudioClip clip)
      {
        Player.clip = clip;
        Player.Play();
      }


      public static void PlayEffect(AudioClip clip)
      {
        Instance.Play(clip);
      }


    }
  }

}