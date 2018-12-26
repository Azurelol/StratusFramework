using System;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus.UI
{
	/// <summary>
	/// Provides various visual effects onto a screen-space UI overlay
	/// </summary>
	[ExecuteInEditMode]
	[Singleton(instantiate = false, persistent = false)]
	public class ScreenOverlay : Singleton<ScreenOverlay>
	{
		public abstract class ScreenOverlayEvent : Stratus.StratusEvent
		{
			public float duration;
			public System.Action onFinished;
		}

		[Serializable]
		public class CrossFadeEvent : ScreenOverlayEvent
		{
			public float alpha = 1.0f;

			public CrossFadeEvent(float alpha, float duration, System.Action onFinished = null)
			{
				this.alpha = alpha;
				this.duration = duration;
				this.onFinished = onFinished;
			}
		}

		public Image screen;
		public bool hideInEditor = true;
		[Header("Startup")]
		public bool fadeIn = true;
		[Tooltip("Default duration for fades")]
		public float duration = 1.5f;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			this.screen.enabled = true;

			if (this.fadeIn)
			{
				this.screen.color = this.screen.color.ToAlpha(1f);
				this.ApplyCrossFade(0f, this.duration, null);
			}

			Scene.Connect<CrossFadeEvent>(this.OnCrossFadeEvent);
		}

		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				return;
			}

			if (this.hideInEditor && this.screen)
			{
				this.screen.enabled = false;
			}
		}

		private void OnDisable()
		{
			if (Application.isPlaying)
			{
				return;
			}

			if (this.hideInEditor && this.screen)
			{
				this.screen.enabled = true;
			}
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void OnCrossFadeEvent(CrossFadeEvent e)
		{
			this.ApplyCrossFade(e.alpha, e.duration, e.onFinished);
		}

		//------------------------------------------------------------------------/
		// Methods: Private
		//------------------------------------------------------------------------/
		private void ApplyCrossFade(float alpha, float duration, System.Action onFinished)
		{
			this.StartCoroutine(Routines.CrossFadeAlpha(this.screen, alpha, duration), "CrossFade", onFinished);
		}
		
		public void FadeIn()
		{
			this.ApplyCrossFade(0f, this.duration, null);
		}

		public void FadeOut()
		{
			this.ApplyCrossFade(0f, this.duration, null);
		}

		//------------------------------------------------------------------------/
		// Methods: Static
		//------------------------------------------------------------------------/
		public static void CrossFade(float alpha, float duration, System.Action onFinished = null)
		{
			Scene.Dispatch<CrossFadeEvent>(new CrossFadeEvent(alpha, duration, onFinished));
		}


	}
}