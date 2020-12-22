using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Stratus.UI
{
	public class StratusDropdown : TMP_Dropdown
	{
		//--------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------/
		public class SelectionEvent : UnityEvent<string>
		{
		}

		//--------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------/
		public SelectionEvent onSelect { get; set; }
		public SelectionEvent onDeselect { get; set; }
		public SelectionEvent onClick { get; set; }

		public string displayedValue => (this.options.Count > 0) ? this.options[this.value].text : string.Empty;

		private bool invalidValue => this.value > (this.options.Count - 1);

		//--------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------/
		protected override void Awake()
		{
			base.Awake();
			this.onSelect = new SelectionEvent();
			this.onDeselect = new SelectionEvent();
			this.onClick = new SelectionEvent();
		}

		public override void OnSelect(BaseEventData eventData)
		{
			if (this.options.Count < 1 || this.invalidValue)
			{
				return;
			}

			base.OnSelect(eventData);
			this.onSelect.Invoke(this.displayedValue);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			if (this.options.Count < 1 || this.invalidValue)
			{
				return;
			}

			base.OnDeselect(eventData);
			this.onDeselect.Invoke(this.displayedValue);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			this.onClick.Invoke(this.displayedValue);
		}



	}

}