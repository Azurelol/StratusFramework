using System.Collections;
using UnityEngine;
using System;

namespace Stratus.UI
{
	public static partial class Extensions
	{
		public static IEnumerator ComposeCrossFade(this CanvasGroup canvasGroup, 
			float alpha,
			bool interactable,
			bool blocksRaycasts,
			float duration,
			Action onFinished = null)
		{
			float initialAlpha = canvasGroup.alpha;

			if (duration > 0.0f)
			{
				yield return StratusRoutines.Lerp((t) =>
				{
					canvasGroup.alpha = initialAlpha.LerpTo(alpha, t);
				}, duration);
			}
			else
			{
				canvasGroup.alpha = alpha;
			}

			canvasGroup.blocksRaycasts = blocksRaycasts;
			canvasGroup.interactable = interactable;

			onFinished?.Invoke();
		}

		public static void CrossFade(this CanvasGroup canvasGroup, 
			float alpha,
			bool interactable,
			bool blocksRaycasts,
			float duration,
			Action onFinished = null)
		{
			StratusCoroutineRunner.Run(canvasGroup.ComposeCrossFade(alpha, interactable, blocksRaycasts, duration, onFinished));
		}

		public static void CrossFade(this CanvasGroup canvasGroup, bool fade, Action onFinished = null)
		{
			float value = fade ? 1f : 0f;
			canvasGroup.alpha = value;
			canvasGroup.interactable = canvasGroup.blocksRaycasts = fade;
			onFinished?.Invoke();
		}

		public static void CrossFade(this CanvasGroup canvasGroup, bool fade, float duration, Action onFinished = null)
		{
			canvasGroup.CrossFade(fade ? 1f : 0f, fade, fade, duration, onFinished);
		}

	}
}