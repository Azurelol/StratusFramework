using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Stratus
{
	public interface IStratusInputLayerProvider
	{
		bool pushInputLayer { get; }
	}

	public abstract class StratusInputLayer : IStratusLogger
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public class PushEvent : StratusEvent
		{
			public StratusInputLayer layer;

			public PushEvent(StratusInputLayer layer)
			{
				this.layer = layer;
			}
		}

		public class PopEvent : StratusEvent
		{
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// An identifier for this input layer
		/// </summary>
		public string label { get; set; }
		/// <summary>
		/// Whether this input layer blocks the pushes of additional input layers until its been popped
		/// </summary>
		public bool blocking { get; set; }
		/// <summary>
		/// Whether this layer should ignore being blocked by an existing blocking layer
		/// </summary>
		public bool ignoreBlocking { get; set; }
		/// <summary>
		/// Whether this input layer is currently pushed. 
		/// If this layer has been made the topmost: if it's enabled it will be made active,
		/// otherwise inactive and will be removed.
		/// </summary>
		public bool pushed { get; internal set; }
		/// <summary>
		/// Whether this input layer has been made active
		/// </summary>
		public bool active
		{
			get => _active;
			internal set
			{
				if (value != _active)
				{
					_active = value;
					this.Log($"{this} has been made {(value ? "active" : "inactive")}");
					onActive?.Invoke(value);
					OnActive(value);
				}
			}
		}
		private bool _active;
		/// <summary>
		/// The input action map this layer is for
		/// </summary>
		public abstract string map { get; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Invoked whenever this input layer is (de)activated
		/// </summary>
		public event Action<bool> onPushed;
		/// <summary>
		/// Invoked whenever this layer has been made active
		/// </summary>
		public event Action<bool> onActive;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusInputLayer(string label)
		{
			this.label = label;
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public override string ToString() => $"{label} ({map})";
		public abstract bool HandleInput(InputAction.CallbackContext context);
		protected abstract void OnActive(bool enabled);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Activates this input layer by sending a scene-level event that will be received by 
		/// any applicable Stratus Player Input behaviour
		/// </summary>
		public void PushByEvent()
		{
			if (pushed)
			{
				this.LogWarning($"Layer {this} already enabled");
				return;
			}
			this.Log($"Pushing layer {this}");
			pushed = true;
			onPushed?.Invoke(true);
			PushEvent e = new PushEvent(this);
			StratusScene.Dispatch<PushEvent>(e);
		}

		/// <summary>
		/// Deactivates this input layer by sending a scene-level event that will be received by 
		/// any applicable Stratus Player Input behaviour.
		/// If this input layer is not at the top, it will be popped from the input stack
		/// when the ones above it have been.
		/// </summary>
		public void PopByEvent()
		{
			if (!pushed)
			{
				StratusDebug.LogError($"Cannot pop disabled layer {label}");
				return;
			}
			this.Log($"Popping layer {this}");
			pushed = false;
			onPushed?.Invoke(false);
			PopEvent e = new PopEvent();
			StratusScene.Dispatch<PopEvent>(e);
		}

		/// <summary>
		/// Activates/deactivates the input layer
		/// </summary>
		/// <param name="toggle"></param>
		public void ToggleByEvent(bool toggle)
		{
			if (toggle)
			{
				PushByEvent();
			}
			else
			{
				PopByEvent();
			}
		}

	}

	public abstract class StratusInputLayer<ActionMap> : StratusInputLayer
		where ActionMap : StratusInputActionMap, new()
	{
		public ActionMap actions { get; } = new ActionMap();
		public override string map => actions.map;

		public StratusInputLayer(string label) : base(label)
		{
		}

		public StratusInputLayer(string label, ActionMap actions) : this(label)
		{
			this.actions = actions;
		}

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			return actions.HandleInput(context);
		}
	}



}