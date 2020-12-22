using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace Stratus.UI
{
	public abstract class StratusDialogRequest : StratusEvent
	{
		public string title { get; set; }
		public Action onOpen { get; set; }
		public Action onClose { get; set; }

		public override string ToString()
		{
			return title;
		}

		protected StratusDialogRequest(string title)
		{
			this.title = title;
		}

		public abstract void Submit();
	}

	public class StratusDialogConfirmationRequest : StratusDialogRequest
	{
		public string message;
		public string confirmText = "OK";
		public string cancelText = "Cancel";
		public Action<bool> onConfirm;

		public StratusDialogConfirmationRequest(string title, string message, Action<bool> onConfirm) 
			: base(title)
		{
			this.message = message;
			this.onConfirm = onConfirm;
		}

		public override void Submit()
		{
			StratusScene.Dispatch<StratusDialogConfirmationRequest>(this);
		}
	}

	public class StratusDialogWindow : StratusCanvasWindow<StratusDialogWindow>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private StratusDialogConfirmationDisplay confirmationRequestPrefab = null;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		private Queue<StratusDialogWindowRequestBehaviour> displays = new Queue<StratusDialogWindowRequestBehaviour>();
		public StratusDialogWindowRequestBehaviour currentRequestDisplay => displays.Peek();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowAwake()
		{
			StratusScene.Connect<StratusDialogConfirmationRequest>(this.OnStratusDialogConfirmationRequest);
		}
		protected override void OnWindowOpen()
		{
		}

		protected override void OnWindowClose()
		{
		}

		protected override StratusInputUILayer GetInputLayer()
		{
			StratusInputUILayer layer = base.GetInputLayer();
			layer.actions.onCancel = CancelCurrentRequest;
			return layer;
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void OnStratusDialogConfirmationRequest(StratusDialogConfirmationRequest e)
		{
			this.Log($"Processed request {e}");
			StratusDialogConfirmationDisplay instance = Instantiate(confirmationRequestPrefab, this.canvas.transform);
			instance.transform.SetAsFirstSibling();
			instance.Open(e, OnRequestEnded);
			instance.Select();
			ProcessRequest(instance);
		}

		//------------------------------------------------------------------------/
		// Requests
		//------------------------------------------------------------------------/
		private void ProcessRequest(StratusDialogWindowRequestBehaviour instance)
		{
			displays.Enqueue(instance);
			if (!open)
			{
				Open();
			}
		}

		private void OnRequestEnded()
		{
			if (this.displays.NotEmpty())
			{
				StratusDialogWindowRequestBehaviour request = displays.Dequeue();
				request.DestroyGameObject();
			}
			if (this.displays.Empty())
			{
				Close();
			}
		}

		private void CancelCurrentRequest()
		{
			if (currentRequestDisplay == null)
			{
				this.LogError("No request?");
			}
			else
			{
				this.Log("Cancelling current request");
			}
			currentRequestDisplay?.Cancel();
		}

	}

}