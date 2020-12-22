using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace Stratus.UI
{
	public class StratusSlideshowBehaviour : StratusBehaviour
	{
		[SerializeField]
		private float transitionDuration = 0.5f;
		[SerializeField]
		private float slideTransitionDuration = 3f;
		[SerializeField]
		private StratusSlideshowAsset asset;

		[SerializeField]
		private CanvasGroup canvasGroup;
		[SerializeField]
		private TextMeshProUGUI textComponent;
		[SerializeField]
		private Image imageComponent;
		[SerializeField]
		private Image backgroundComponent;

		[SerializeField]
		private bool ignoreTimeScale = true;

		public int currentSlideNumber { get; private set; }
		private Coroutine advanceRoutine { get; set; }
		public bool active => advanceRoutine != null;

		private void Awake()
		{
			Stop();
		}

		public void Play(Action onFinished = null)
		{
			if (asset == null)
			{
				this.LogError("No assigned slideshow asset");
				return;
			}

			if (active)
			{
				this.LogError("Already running!");
				return;
			}

			this.Log("Playing");
			void finished()
			{
				onFinished?.Invoke();
				advanceRoutine = null;
			}
			advanceRoutine = StartCoroutine(AdvanceRoutine(finished));
		}

		public void Stop()
		{
			this.Log("Stopping");
			if (active)
			{
				StopCoroutine(advanceRoutine);
			}
			backgroundComponent.CrossFadeAlpha(0f, 0f, ignoreTimeScale);
			canvasGroup.CrossFade(false);
			currentSlideNumber = 0;
			textComponent.text = string.Empty;
			imageComponent.sprite = null;
			advanceRoutine = null;
		}

		private IEnumerator AdvanceRoutine(Action onFinished)
		{
			// Fade in
			this.Log("Starting slide show...");
			backgroundComponent.CrossFadeAlpha(1f, transitionDuration / 2f, ignoreTimeScale);
			yield return new WaitForSeconds(transitionDuration);

			// Set the slides
			for (int i = 0; i < asset.slides.Length; i++)
			{
				currentSlideNumber = i;
				StratusSlideshowAsset.Slide slide = asset.slides[i];

				// Set
				textComponent.text = slide.text;
				imageComponent.sprite = slide.sprite;

				float transition = slide.transition > 0.0f 
					? slide.transition : slideTransitionDuration;

				// Fade in
				canvasGroup.CrossFade(true, transition);
				yield return new WaitForSeconds(transition);

				// Fade out
				canvasGroup.CrossFade(false, transition);
				yield return new WaitForSeconds(transition);
			}

			this.Log("Ending slide show...");
			yield return new WaitForSeconds(transitionDuration / 2f);
			backgroundComponent.CrossFadeAlpha(0f, transitionDuration, ignoreTimeScale);
			yield return new WaitForSeconds(transitionDuration / 2f);
			onFinished?.Invoke();
		}

		private void Set(StratusSlideshowAsset.Slide slide)
		{
			textComponent.text = slide.text;
			imageComponent.sprite = slide.sprite;
		}

	}

}