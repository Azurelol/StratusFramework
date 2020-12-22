using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Stratus.UI
{
	public abstract class StratusDialogWindowRequestBehaviour : StratusCanvasGroupBehaviour
	{
		public abstract void Select();
		public abstract void Confirm();
		public abstract void Cancel();
	}

	public abstract class StratusDialogWindowRequestDisplay<T> : StratusDialogWindowRequestBehaviour 
		where T : StratusDialogRequest
	{
		public TextMeshProUGUI titleText;
		public Button closeButton;

		public LayoutGroup buttonGroup;
		public Button buttonPrefab;

		private List<Button> buttons = new List<Button>();

		public T request { get; private set; }

		private Action onClose { get; set; }

		public virtual void Open(T request, Action onClose)
		{
			this.request = request;	
			this.titleText.text = request.title;
			this.onClose = onClose;
			closeButton.onClick.AddListener(onClose);
			request.onOpen?.Invoke();
		}

		public void Close()
		{
			request.onClose?.Invoke();
			onClose();
		}

		public override void Select()
		{
			if (buttons.NotEmpty())
			{
				buttons.First().Select();
			}
		}

		public void SpawnButtons(params StratusLabeledAction[] actions)
		{
			foreach(StratusLabeledAction action in actions)
			{
				Button instance = GameObject.Instantiate(buttonPrefab, buttonGroup.transform);
				SetButton(instance, action.label, action.action);
				buttons.Add(instance);
			}

			StratusCanvasUtility.SetExplicitNavigation(buttons, x => x, StratusOrientation.Horizontal);

		}

		private void SetButton(Button instance, string name, Action action)
		{
			instance.GetComponentInChildren<TextMeshProUGUI>().text = name;
			instance.onClick.AddListener(action);
		}

		public override void Confirm()
		{
			Close();
		}

		public override void Cancel()
		{
			Close();
		}
	}

	public class StratusDialogConfirmationDisplay : StratusDialogWindowRequestDisplay<StratusDialogConfirmationRequest>
	{
		public TextMeshProUGUI messageText;

		public override void Open(StratusDialogConfirmationRequest request, Action onClose)
		{
			base.Open(request, onClose);
			messageText.text = request.message;
			SpawnButtons(new StratusLabeledAction(request.confirmText, Confirm),
				new	StratusLabeledAction(request.cancelText, Cancel));
		}

		private void ConfirmAction(bool confirm)
		{
			Close();
			request.onConfirm(confirm);
		}

		public override void Confirm()
		{
			ConfirmAction(true);
		}

		public override void Cancel()
		{
			ConfirmAction(false);
		}
	}
}