using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace Stratus.UI
{	
	public abstract class StratusPagedBehaviourDisplay<BehaviourType> : StratusCanvasGroup
		where BehaviourType : Behaviour
	{
		[SerializeField]
		protected bool closeOnLastPage = false;

		private BehaviourType[] pages;
		private StratusArrayNavigator<BehaviourType> navigator;

		public int currentPageNumber => navigator.currentIndex;
		public int lastPageNumber => navigator.lastIndex;

		protected abstract void TogglePage(BehaviourType page, bool show);

		protected override void OnInitialize()
		{
			pages = canvasGroup.GetComponentsInChildrenNotIncludeSelf<BehaviourType>(true);
			navigator = new StratusArrayNavigator<BehaviourType>(pages, false);
			navigator.onChanged += this.OnPageChanged;

			foreach (var page in pages)
			{
				TogglePage(page, false);
			}
		}

		protected override StratusInputUILayer GetInputLayer()
		{
			void Navigate(Vector2 dir)
			{
				navigator.Navigate(dir);
				if (navigator.recentlyChanged && closeOnLastPage)
				{
					Hide();
				}
			}

			void OnSubmit()
			{
				if (navigator.atLastIndex)
				{
					Hide();
				}
				else
				{
					navigator.Next();
				}
			}

			var layer = base.GetInputLayer();
			layer.actions.onNavigate = Navigate;
			layer.actions.onSubmit = OnSubmit;
			return layer;
		}

		protected override void OnShow()
		{
			base.OnShow();
			navigator.NavigateToFirst();
			if (!navigator.recentlyChanged)
			{
				TogglePage(navigator.current, true);
				UpdatePageCounter();
			}
		}

		protected override void OnHide()
		{
			base.OnHide();
		}

		private void OnPageChanged(BehaviourType current, BehaviourType previous)
		{
			TogglePage(current, true);
			if (previous != null)
			{
				TogglePage(previous, false);
			}
			UpdatePageCounter();
		}

		protected abstract void UpdatePageCounter();
	}

	[Serializable]
	public class StratusPagedCanvasGroupDisplay : StratusPagedBehaviourDisplay<CanvasGroup>
	{
		[SerializeField]
		private TextMeshProUGUI pageCounterText;

		protected override void TogglePage(CanvasGroup page, bool show)
		{
			this.Log($"Toggling page {page} ({currentPageNumber}) ? {show}");
			if (show)
			{
				page.CrossFade(1f, true, true, fadeDuration);
			}
			else
			{
				page.CrossFade(0f, false, false, fadeDuration);
			}
		}

		protected override void UpdatePageCounter()
		{
			pageCounterText.text = $"{currentPageNumber+ 1}/{lastPageNumber + 1}";
		}
	}
}