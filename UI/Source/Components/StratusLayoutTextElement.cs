using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stratus.UI
{
	[Serializable]
	public class StratusLayoutTextElementEntry : StratusLayoutElementEntry
	{
		public StratusValue<Sprite> icon;

		public StratusLayoutTextElementEntry()
		{
		}

		public StratusLayoutTextElementEntry(string label) : base(label)
		{
		}

		public StratusLayoutTextElementEntry(string label, Action action) : base(label, action)
		{
		}

		public StratusLayoutTextElementEntry(StratusLabeledAction labeledAction) : this(labeledAction.label, labeledAction.action)
		{
		}
	}

	[Serializable]
	public class StratusTextStyle
	{
		public int fontSize = 12;
		public bool richText = true;
		public bool autoSizeText = false;
		public TextAlignmentOptions textAlignment = TextAlignmentOptions.MidlineGeoAligned;
	}

	[Serializable]
	public class StratusLayoutTextElementStyle : StratusLayoutElementStyle
	{
		public StratusTextStyle textStyle = new StratusTextStyle();
	}

	/// <summary>
	/// An layout entry that uses a text component 
	/// </summary>
	public class StratusLayoutTextElement : StratusLayoutElement<TextMeshProUGUI,
		StratusLayoutTextElementEntry, StratusLayoutTextElementStyle>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private Image iconComponent;
		[SerializeField]
		private RectTransform iconFrame;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public string text
		{
			get => this.body.text;
			set => this.body.text = value;
		}

		public override Color bodyColor
		{
			get => this.body.color;
			protected set => this.body.color = value;
		}

		public Sprite icon
		{
			get => iconComponent.sprite;
			set => iconComponent.sprite = value;
		}

		public bool hasIcon => icon != null;

		protected override void OnUpdateContent(StratusLayoutTextElementEntry entry)
		{
			text = entry.label;
			if (entry.icon != null)
			{
				this.icon = entry.icon.value;
				iconComponent.enabled = hasIcon;
				iconFrame.gameObject.SetActive(hasIcon);
				iconFrame.SetWidth(style.bodyHeight);
			}
		}

		protected override void OnInitialize(StratusLayoutTextElementEntry entry, StratusLayoutTextElementStyle style)
		{
			if (style != null)
			{
				if (style.textStyle.autoSizeText)
				{
					body.enableAutoSizing = true;
				}
				else
				{
					if (style.textStyle.fontSize != default)
					{
						body.fontSize = style.textStyle.fontSize;
					}
				}
				body.richText = style.textStyle.richText;
				body.alignment = style.textStyle.textAlignment;
			}
		}
	}

}