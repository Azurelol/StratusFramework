using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace Stratus.UI
{
	/// <summary>
	/// When added to a monobehaviour, will show a tooltip when the cursor
	/// hovers it
	/// </summary>
	public class StratusTooltipBehaviour : StratusBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private string _tooltip;
		public string tooltip => _tooltip.IsValid() ? _tooltip : gameObject.name;

		public static StratusTooltipBehaviour current
		{
			get => _current;
			set
			{
				if (_current != value)
				{
					_current = value;
					onTooltipChanged?.Invoke(_current);
				}
			}
		}
		private static StratusTooltipBehaviour _current;

		public static event Action<StratusTooltipBehaviour> onTooltipChanged;

		public bool isCanvasObject { get; private set; }

		private void Awake()
		{
			isCanvasObject = this.gameObject.HasComponent<RectTransform>();
		}

		private void OnMouseEnter()
		{
			Assign();
		}

		private void OnMouseExit()
		{
			Unassign();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			Assign();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Unassign();
		}

		private void Assign()
		{
			current = this;
		}

		private void Unassign()
		{
			if (current == this)
			{
				current = null;
			}
		}
	}

}