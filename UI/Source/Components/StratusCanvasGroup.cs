using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Stratus.UI
{
	[Serializable]
	public class StratusCanvasGroup : IStratusLogger
	{
		public CanvasGroup canvasGroup;
		public float fadeDuration = 0f;
		public bool pushInputLayer = false;
		public Selectable selectable = null;

		public StratusInputUILayer inputLayer { get; private set; }

		public event Action<bool> onShow;

		public bool initialized { get; private set; }
		public bool debug { get; set; }

		public void Toggle(bool show)
		{
			if (debug)
			{
				this.Log($"Show? {show}");
			}

			if (canvasGroup != null)
			{
				Initialize();
				// SHOW
				if (show)
				{
					if (pushInputLayer)
					{
						inputLayer.PushByEvent();
					}
					OnShow();
					onShow?.Invoke(true);
					if (selectable != null)
					{
						selectable.Select();
					}
				}
				// HIDE
				else
				{
					if (pushInputLayer)
					{
						inputLayer.PopByEvent();
					}
					OnHide();
					onShow?.Invoke(false);
				}
				ToggleVisibility(show, fadeDuration);
			}
		}

		public void Show() => Toggle(true);
		public void Hide() => Toggle(false);

		public void Initialize()
		{
			if (initialized)
			{
				return;
			}
			if (!canvasGroup.gameObject.activeSelf)
			{
				canvasGroup.gameObject.SetActive(true);
			}
			ToggleVisibility(false, 0f);
			OnInitialize();
			inputLayer = GetInputLayer();
			initialized = true;
		}

		private void ToggleVisibility(bool show, float duration)
		{
			Action onFinished = null;
			if (show)
			{
				canvasGroup.gameObject.SetActive(true);
			}
			else
			{
				onFinished = () => canvasGroup.gameObject.SetActive(false);
			}
			canvasGroup.CrossFade(show ? 1f : 0f, show, show, duration, onFinished);
		}

		protected virtual StratusInputUILayer GetInputLayer()
		{
			inputLayer = new StratusInputUILayer(GetType().Name); ;
			inputLayer.actions.onCancel = Hide;
			return inputLayer;
		}

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnEnabled()
		{
		}

		protected virtual void OnDisabled()
		{
		}

		protected virtual void OnHide()
		{
		}

		protected virtual void OnShow()
		{
		}

	}

	

	[RequireComponent(typeof(CanvasGroup))]
	public abstract class StratusCanvasGroupBehaviour : StratusBehaviour
	{
		public CanvasGroup canvasGroup => GetComponentCached<CanvasGroup>();
	}

	[Serializable]
	public class StratusTransformReferenceMap
	{
		[SerializeField]
		private List<Transform> _references = new List<Transform>();

		private Dictionary<string, Transform> referenceMap
		{
			get
			{
				if (_referenceMap == null)
				{
					_referenceMap = new Dictionary<string, Transform>();
					foreach (var reference in _references)
					{
						if (!_referenceMap.AddUnique(reference.name, reference))
						{
							StratusDebug.LogWarning($"Duplicate key {reference.name}");
						}
					}
				}
				return _referenceMap;
			}
		}
		private Dictionary<string, Transform> _referenceMap;

		public ComponentType GetReferencedComponent<ComponentType>(string transformName)
			where ComponentType : Component
		{
			if (!referenceMap.ContainsKey(transformName))
			{
				return null;
			}
			return referenceMap[transformName].GetComponent<ComponentType>();
		}
	}

	public class StratusCanvasEventCatcher
	{
		public GameObject instance { get; private set; }
		public RectTransform parent { get; private set; }
		public Action onClick { get; private set; }

		public StratusCanvasEventCatcher(RectTransform parent, Action onClick, bool enabled)
		{
			this.parent = parent;
			this.onClick = onClick;

			this.instance = new GameObject($"{parent} Canvas Event Catcher");
			this.instance.transform.SetParent(parent);
			Image image = this.instance.AddComponent<Image>();
			image.color = Color.clear;
			Button button = this.instance.AddComponent<Button>();
			button.onClick.AddListener(onClick);

			Toggle(enabled);
		}

		public void Toggle(bool toggle) => instance.SetActive(toggle);
	}

}