
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace Stratus.UI
{
	public class StratusTooltipCanvas : StratusCanvasBehaviour
	{
		[SerializeField]
		private Image tooltipFrame;
		[SerializeField]
		private TextMeshProUGUI tooltipText;
		[SerializeField]
		private Vector2 cursorOffset = new Vector2(16f, -16f);

		public bool active { get; private set; }
		public string tooltip { get; private set; }

		private void Awake()
		{
			StratusTooltipBehaviour.onTooltipChanged += this.OnTooltipChanged;
			Toggle(false);
		}

		private void Update()
		{
			if (active)
			{
				Reposition();
			}
		}

		private void OnTooltipChanged(StratusTooltipBehaviour behaviour)
		{
			if (behaviour != null)
			{
				tooltipText.text = tooltip = behaviour.tooltip;
				Reposition();
				Toggle(true);
			}
			else
			{
				tooltip = null;
				Toggle(false);
			}
		}

		private void Reposition()
		{
			Vector3 position = Input.mousePosition;
			position.x += cursorOffset.x;
			position.y += cursorOffset.y;
			tooltipFrame.rectTransform.position = position;
		}

		private void Toggle(bool toggle)
		{
			tooltipFrame.enabled = tooltipText.enabled = toggle;
			active = toggle;
		}
	}

}