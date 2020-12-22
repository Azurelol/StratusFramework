using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Stratus;
using System.Data;

namespace Stratus.UI
{
	/// <summary>
	/// A pop up text that is displayed over a transform in the scene
	/// </summary>
	[Serializable]
	public class StratusPopUpTransformText
	{
		public class InstantiateEvent : StratusEvent
		{
			public StratusPopUpTransformText popUpText { get; private set; }
			public InstantiateEvent(StratusPopUpTransformText popUpText)
			{
				this.popUpText = popUpText;
			}

		}

		public Transform transform;
		public Vector3 offset;

		public string text;
		public bool richText;
		public int fontSize = defaultFontSize;
		public Color color = Color.white;
		[Min(0.1f)]
		public float duration = defaultDuration;
		public float fadeOutDuration = 0f;

		public const int defaultFontSize = 14;
		public const float defaultDuration = 0.5f;

		public bool instanced { get; internal set; }

		public StratusPopUpTransformText(Transform transform, string text)
		{
			this.transform = transform;
			this.text = text;
		}

		public StratusPopUpTransformText(Transform transform, object obj)
		{
			this.transform = transform;
			this.text = obj.ToString();
		}

		public override string ToString()
		{
			return $"Transform({transform}), Text({text})";
		}

		public bool Instantiate()
		{
			if (instanced)
			{
				return false;
			}
			StratusScene.Dispatch<InstantiateEvent>(new InstantiateEvent(this));
			return true;
		}
	}

	/// <summary>
	/// Default canvas behaviour that manages pop up text requests for transforms
	/// </summary>
	[StratusSingleton(instantiate = false)]
	public class StratusPopUpTransformTextCanvas : StratusCanvasBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private TextMeshPro textComponentPrefab;
		[SerializeField]
		private Camera outputCamera;
		[SerializeField]
		private bool debug = false;

		private StratusBehaviourPool<TextMeshPro, StratusPopUpTransformText> textObjectPool;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			textObjectPool = new StratusBehaviourPool<TextMeshPro, StratusPopUpTransformText>(this.transform, textComponentPrefab, OnInstantiate);
			textObjectPool.debug = debug;
			StratusScene.Connect<StratusPopUpTransformText.InstantiateEvent>(this.OnPopUpTextInstantiateEvent);
		}

		private void Update()
		{
			if (textObjectPool.instantiated)
			{
				textObjectPool.Update(UpdateText);
			}
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		private void OnPopUpTextInstantiateEvent(StratusPopUpTransformText.InstantiateEvent e)
		{
			OnTextAdded(e.popUpText);
		}

		private void OnTextRemoved(StratusPopUpTransformText obj)
		{
			bool removed = textObjectPool.Remove(obj);
			obj.instanced = !removed;
			if (!removed)
			{
				this.LogError($"Failed to remove {obj}");
			}
		}

		private void OnTextAdded(StratusPopUpTransformText obj)
		{
			textObjectPool.Instantiate(obj);
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		public static void Instantiate(StratusPopUpTransformText popUpText)
		{
			StratusScene.Dispatch<StratusPopUpTransformText.InstantiateEvent>(new StratusPopUpTransformText.InstantiateEvent(popUpText));
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void OnInstantiate(TextMeshPro instance, StratusPopUpTransformText parameters)
		{
			//Vector3 screenPosition = Camera.main.WorldToScreenPoint(parameters.transform.position);
			if (debug)
			{
				this.Log($"Instantiating text {parameters}");
			}

			UpdateText(instance, parameters);
			instance.text = parameters.text ?? string.Empty;
			instance.alpha = 1f;
			instance.richText = parameters.richText;
			instance.color = parameters.color;

			instance.fontSize = parameters.fontSize > 0 ? parameters.fontSize : StratusPopUpTransformText.defaultFontSize;
			if (parameters.duration <= 0.0f)
			{
				this.LogWarning($"Invalid duration of {parameters.duration} set. Using default of {StratusPopUpTransformText.defaultDuration}");
				parameters.duration = StratusPopUpTransformText.defaultDuration;
			}
			parameters.instanced = true;

			// Remove after a set amount of time
			Action removeAction = () =>
			{
				OnRemoveAction(instance, parameters);
				OnTextRemoved(parameters);
			};
			Invoke(removeAction, parameters.duration + parameters.fadeOutDuration);
		}

		private void UpdateText(TextMeshPro text, StratusPopUpTransformText data)
		{
			Vector3 position = data.transform.position + data.offset;
			text.transform.position = position; 
			text.transform.forward = position - outputCamera.transform.position;
		}

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected virtual void OnRemoveAction(TextMeshPro instance, StratusPopUpTransformText parameters)
		{
			if (parameters.fadeOutDuration > 0.0f)
			{
				instance.CrossFadeAlpha(0f, parameters.fadeOutDuration, true);
			}
		}


	}

}