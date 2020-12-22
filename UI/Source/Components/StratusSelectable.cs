using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Stratus.UI
{
	public interface IStratusSelectableParent<SelectableType>
		where SelectableType : StratusSelectable
	{
		SelectableType activeSelectable { get; }
		void OnSelected(SelectableType selectable);
		void OnDeselected(SelectableType selectable);
	}

	public interface IStratusSelectableParent : IStratusSelectableParent<StratusSelectable>
	{
	}

	public abstract class StratusSelectable : StratusBehaviour, IStratusBehaviour, IStratusInputUIActionHandler
	{
		//--------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------/
		/// <summary>
		/// A reference to the rect transform this component uses
		/// </summary>
		public RectTransform rectTransform { get { return GetComponentCached<RectTransform>(); } }
		/// <summary>
		/// The object that is managing this selectable
		/// </summary>
		public IStratusSelectableParent handler { get; private set; }
		/// <summary>
		/// The selectable this component uses
		/// </summary>
		public abstract Selectable selectable { get;  }
		/// <summary>
		/// Whether the selectable this represents is selected
		/// </summary>
		public bool selected { get; private set; }

		//--------------------------------------------------------------------------/
		// Events
		//--------------------------------------------------------------------------/
		public event Action<StratusSelectable> onSelected;
		public event Action<StratusSelectable> onDeselected;

		//--------------------------------------------------------------------------/
		// Virtual
		//--------------------------------------------------------------------------/
		protected abstract void OnSelectableAwake();
		public abstract void Navigate(Vector2 dir);

		//--------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------/\
		private void Awake()
		{
			selectable.AddEventTrigger(EventTriggerType.Select, OnSelect);
			selectable.AddEventTrigger(EventTriggerType.Deselect, OnDeselect);
		}

		void OnSelect(BaseEventData eventData)
		{
			selected = true;
			onSelected?.Invoke(this);
			handler?.OnSelected(this);
		}

		void OnDeselect(BaseEventData eventData)
		{
			selected = false;
			onDeselected?.Invoke(this);
			handler?.OnDeselected(this);
		}

		//--------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------/
		public void SetParent(IStratusSelectableParent handler)
		{
			this.handler = handler;
		}

		public void Select()
		{
			selectable.Select();
		}

		public void Deselect()
		{
			selectable.OnDeselect(null);
		}

	}

}