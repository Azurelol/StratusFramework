using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Stratus
{
	public enum StratusPlayerInputActionMap
	{
		Player,
		UI
	}

	public enum StratusInputScheme
	{
		Unknown,
		KeyboardMouse,
		DualShock,
		Xbox
	}

	public interface IStratusPlayerInput
	{
		event Action<StratusInputScheme> onInputSchemeChanged;
		StratusInputActionDecorator GetActionDecorator(string action);
	}

	/// <summary>
	/// Base class for inputs that work with Unity's newer InputSystem
	/// </summary>
	[StratusSingleton(instantiate = false)]
	public abstract class StratusPlayerInput<T> : StratusSingletonBehaviour<T>, IStratusPlayerInput
		where T : StratusSingletonBehaviour<T>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public PlayerInput playerInput;
		public StratusInputActionDecoratorMap decoratorMap;
		public bool debug = false;
		public bool logInputCallback = false;

		private StratusInputStack<StratusInputLayer> inputLayers = new StratusInputStack<StratusInputLayer>();
		private Dictionary<string, StratusInputScheme> inputSchemes = new Dictionary<string, StratusInputScheme>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public InputActionMap currentActionMap => playerInput.currentActionMap;
		public StratusInputLayer currentInputLayer => inputLayers.activeLayer;
		public string currentActionMapName => playerInput.currentActionMap.name;
		public bool hasInputLayer => inputLayers.layerCount > 0;
		public bool inputEnabled
		{
			get => playerInput != null ? playerInput.enabled : false;
			set
			{
				if (playerInput != null)
				{
					if (debug)
					{
						this.Log($"{(value ? "Enabling" : "Disabling")} player input");
					}
					playerInput.enabled = value;
				}
			}
		}
		public StratusInputScheme latestInputSchemeUsed { get; private set; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public event Action<StratusInputScheme> onInputSchemeChanged;

		//------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------/
		protected abstract void OnInputAwake();
		protected abstract void OnInputSchemeChanged(StratusInputScheme inputScheme);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			StratusScene.Connect<StratusInputLayer.PushEvent>(OnPushLayerEvent);
			StratusScene.Connect<StratusInputLayer.PopEvent>(OnPopLayerEvent);
			playerInput.onActionTriggered += OnInputActionTriggered;
			playerInput.onControlsChanged += this.OnControlsChanged;
			inputLayers.onInputLayerChanged += this.OnInputLayerChanged;
			RecordInputDevices();
			OnInputAwake();
		}

		private void OnControlsChanged(PlayerInput obj)
		{
		}

		private void Reset()
		{
			playerInput = GetComponent<PlayerInput>();
		}

		private void OnGUI()
		{
			if (debug)
			{
				if (inputLayers.hasActiveLayers)
				{
					StratusGUI.GUILayoutArea(StratusGUI.Anchor.TopRight, StratusGUI.quarterScreen, (Rect rect) =>
					{
						GUILayout.Label(inputLayers.activeLayer.label, StratusGUIStyles.headerWhite);
					});
				}
			}
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void OnPushLayerEvent(StratusInputLayer.PushEvent e)
		{
			StratusValidation result = inputLayers.Push(e.layer);
			if (debug)
			{
				this.Log($"Pushed input layer: {e.layer.label}. Result ? {result}");
			}
		}

		private void OnPopLayerEvent(StratusInputLayer.PopEvent e)
		{
			// Pop layers that have been marked as inactive
			while (inputLayers.canPop)
			{
				StratusInputLayer layer = inputLayers.Pop();
				if (debug)
				{
					if (layer != null)
					{
						this.Log($"Removed input layer: {layer}. Current layer: {inputLayers.activeLayer}");
					}
					else
					{
						this.LogError("No input layer to remove!");
					}
				}
			}
		}

		private void OnInputLayerChanged(StratusInputLayer layer)
		{
			bool switched = false;

			if (!IsCurrentActionMap(layer.map))
			{				
				try
				{
					if (!playerInput.enabled)
					{
						playerInput.enabled = true;
					}
					playerInput.SwitchCurrentActionMap(layer.map);
					if (IsCurrentActionMap(layer.map))
					{
						switched = true;
					}
					else
					{
						this.LogError($"Could not find action map for layer {layer.map}");
					}
				}
				catch (Exception exception)
				{
					this.LogError(exception);
				}
			}

			if (debug)
			{
				if (switched)
				{
					this.Log($"Action map switched to {layer.map}");
				}
				this.Log($"Input layer now '{layer.label}'");
			}
		}

		private bool IsCurrentActionMap(string actionMapName)
		{
			return playerInput.currentActionMap != null && playerInput.currentActionMap.name.Equals(actionMapName, StringComparison.InvariantCultureIgnoreCase);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void ActivateInput()
		{
			playerInput.ActivateInput();
		}

		public void DeactivateInput()
		{
			playerInput.DeactivateInput();
		}

		public StratusInputActionDecorator GetActionDecorator(string action)
		{
			if (decoratorMap == null)
			{
				this.LogWarning("No decorator asset has been assigned");
				return null;
			}

			return decoratorMap.GetDecorator(this.latestInputSchemeUsed, action);
		}

		public Sprite GetActionSprite(string action)
		{
			var decorator = GetActionDecorator(action);
			if (decorator == null)
			{
				return null;
			}
			return decorator.sprite;
		}

		//------------------------------------------------------------------------/
		// Statics
		//------------------------------------------------------------------------/
		public static void DispatchPushLayerEvent(StratusInputLayer layer)
		{
			layer.pushed = true;
			StratusScene.Dispatch<StratusInputLayer.PushEvent>(new StratusInputLayer.PushEvent(layer));
		}

		public static void DispatchPopLayerEvent(StratusInputLayer layer)
		{
			layer.pushed = false;
			StratusScene.Dispatch<StratusInputLayer.PopEvent>(new StratusInputLayer.PopEvent());
		}

		//------------------------------------------------------------------------/
		// Main Input Procedures
		//------------------------------------------------------------------------/
		private void OnInputActionTriggered(InputAction.CallbackContext context)
		{
			if (hasInputLayer)
			{
				// Update the current input device if it has changed
				string deviceName = context.control.device.name;
				if (inputSchemes.ContainsKey(deviceName))
				{
					var device = inputSchemes[deviceName];
					if (latestInputSchemeUsed != device)
					{
						UpdateLatestInputDevice(device);
					}
				}

				// Handle the input
				if (logInputCallback)
				{
					bool handled = currentInputLayer.HandleInput(context);
					this.Log($"[{(handled ? "HANDLED" : "UNHANDLED")}] {context}");
				}
				else
				{
					currentInputLayer.HandleInput(context);
				}
			}
			else
			{
				if (logInputCallback)
				{
					this.LogWarning($"[NO INPUT LAYER] {context}");
				}
			}
		}

		private void RecordInputDevices()
		{
			foreach (InputDevice device in playerInput.devices)
			{
				StratusInputScheme result = TryParse(device.name);
				if (result != StratusInputScheme.Unknown)
				{
					inputSchemes.Add(device.name, result);
				}
				if (debug)
				{
					this.Log($"Device '{device.name}' detected as {result}");
				}
			}
		}

		private void UpdateLatestInputDevice(StratusInputScheme inputScheme)
		{
			latestInputSchemeUsed = inputScheme;
			onInputSchemeChanged?.Invoke(latestInputSchemeUsed);
			OnInputSchemeChanged(latestInputSchemeUsed);
			if (debug)
			{
				this.Log($"Last device used now {latestInputSchemeUsed}");
			}
		}

		public static StratusInputScheme TryParse(string deviceName)
		{
			StratusInputScheme result = StratusInputScheme.Unknown;
			switch (deviceName)
			{
				case "Keyboard":
					result = StratusInputScheme.KeyboardMouse;
					break;

				case "Mouse":
					result = StratusInputScheme.KeyboardMouse;
					break;
			}
			if (result == StratusInputScheme.Unknown)
			{
				if (deviceName.Contains("DualShock"))
				{
					result = StratusInputScheme.DualShock;
				}
			}
			return result;
		}
	}

}