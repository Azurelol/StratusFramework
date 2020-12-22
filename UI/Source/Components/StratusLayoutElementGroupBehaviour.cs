using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stratus.UI
{
	public abstract class StratusLayoutElementGroupBehaviour<ElementType, EntryType, StyleType>
		: StratusBehaviour, ISelectHandler
		where ElementType : StratusLayoutElement, IStratusLayoutElement<EntryType, StyleType>
		where EntryType : StratusLayoutElementEntry
		where StyleType : StratusLayoutElementStyle
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Tooltip("The scroll rect used")]
		public ScrollRect scrollRect;
		[Tooltip("Layout group used")]
		public LayoutGroup layoutGroup;
		[Tooltip("Prefab used for the layout elements")]
		public ElementType prefab;
		[Tooltip("Styling used for this element")]
		public StyleType style;
		[Tooltip("When an input field is added, it allows the filtering of the elements")]
		public TMP_InputField filterInputField;
		[Tooltip("Whether explicit navigation is set between its elements")]
		public bool explicitNavigation = true;
		public StratusOrientation orientation = StratusOrientation.Vertical;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		private StratusFilteredStringList elementFilter { get; set; }
		private List<ElementType> _instances { get; set; }
		private StratusArrayNavigator<ElementType> navigator { get; set; }
		public string filter { get; set; }
		public ElementType[] elements => _instances.ToArray();
		public ElementType selectedInstance { get; private set; }
		public bool hasElements => !_instances.IsNullOrEmpty();
		private ContentSizeFitter contentSizeFitter => contentTransform.GetComponent<ContentSizeFitter>();
		private RectTransform contentTransform => scrollRect.content;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public void Reset()
		{
			if (_instances != null)
			{
				_instances.DestroyGameObjectsAndClear();
			}
			switch (orientation)
			{
				case StratusOrientation.Horizontal:
					contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
					contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
					contentTransform.SetHeight(style.bodyHeight);
					break;
				case StratusOrientation.Vertical:
					contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
					contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
					break;
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public StratusValidation Set(IEnumerable<EntryType> entries)
		{
			if (layoutGroup == null)
			{
				string msg = "No layout group has been set";
				this.LogError(msg);
				return new StratusValidation(false, msg);
			}
			Reset();
			if (_instances == null)
			{
				_instances = new List<ElementType>();
			}
			foreach (EntryType entry in entries)
			{
				ElementType element = Instantiate(prefab, layoutGroup.transform);
				element.Initialize(entry, style);
				element.onSelect += () => OnLayoutElementSelected(element);
				_instances.Add(element);
			}
			navigator = new StratusArrayNavigator<ElementType>(_instances, true);
			if (explicitNavigation)
			{
				SetExplicitNavigation();
			}
			return true;
		}

		public StratusValidation Set(params EntryType[] elements) => Set((IEnumerable<EntryType>)elements);

		public void Refresh()
		{
			for (int i = 0; i < _instances.Count; i++)
			{
				ElementType instance = _instances[i];
				instance.UpdateContent();
			}
		}

		private void SetExplicitNavigation()
		{
			StratusCanvasUtility.SetExplicitNavigation(_instances, x => x.selectable, StratusOrientation.Vertical);
		}

		public void RemoveFirst()
		{
			if (_instances.NotEmpty())
			{
				ElementType first = _instances.First();
				Destroy(first.gameObject);
				_instances.RemoveAt(0);
			}
		}

		void ISelectHandler.OnSelect(BaseEventData eventData)
		{
			Select();
		}

		public void Select()
		{
			if (!hasElements)
			{
				return;
			}
			selectedInstance = navigator.NavigateToFirst();
			selectedInstance?.Select();
		}

		public void SubmitCurrent()
		{
			selectedInstance?.Submit();
		}

		public void Navigate(Vector2 dir)
		{
			if (dir.x > 0 || dir.y < 0)
			{
				SelectNext();
			}
			else if (dir.x < 0 || dir.y > 0)
			{
				SelectPrevious();
			}
		}

		public void SelectNext()
		{
			if (_instances != null)
			{
				selectedInstance = navigator.Next();
				selectedInstance?.Select();
			}
		}

		public void SelectPrevious()
		{
			if (_instances != null)
			{
				selectedInstance = navigator.Previous();
				selectedInstance?.Select();
			}
		}

		public void Clear()
		{
			_instances?.DestroyGameObjectsAndClear();
		}

		public StratusInputUILayer GetInputLayer()
		{
			StratusInputUILayer layer = new StratusInputUILayer(this.gameObject.name);
			layer.actions.onNavigate = Navigate;
			layer.actions.onSubmit = SubmitCurrent;
			return layer;
		}

		private void OnLayoutElementSelected(ElementType layoutElement)
		{
			//scrollRect.ScrollToChildIfNotVisible(layoutElement.rectTransform);
		}

	}
}