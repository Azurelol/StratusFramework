using System;
using UnityEngine;

namespace Stratus.Experimental
{
	[StratusSingleton("Screen Shader Transition", true, false)]
	public class StratusScreenShaderTransition : StratusSingletonBehaviour<StratusScreenShaderTransition>
	{
		//------------------------------------------------------------------------/
		// Event
		//------------------------------------------------------------------------/
		[Serializable]
		public class TransitionEvent : StratusEvent
		{
			/// <summary>
			/// The mask being used for this transition
			/// </summary>
			[Tooltip("The mask being used for this transition")]
			public Texture2D mask;
			/// <summary>
			/// What to set the initial value at.
			/// </summary>
			[Tooltip("What to set the initial value at")]
			[Range(0f, 1f)]
			public float initialValue = 1.0f;
			/// <summary>
			/// The final value.
			/// </summary>
			[Tooltip("What to set the ending value at")]
			[Range(0f, 1f)]
			public float endingValue = 0.0f;
			/// <summary>
			/// How fast the transition takes to complete
			/// </summary>
			[Tooltip("How fast the transition takes to complete")]
			[Range(1f, 5f)]
			public float speed = 3.0f;
			/// <summary>
			/// How long the transition lasts.
			/// </summary>
			[Tooltip("How long the transition effect lasts")]
			[Range(0f, 10f)]
			public float duration = 0.0f;

			public TransitionEvent(Texture2D mask, float initialValue, float endingValue, float speed, float duration)
			{
				this.mask = mask;
				this.initialValue = initialValue;
				this.endingValue = endingValue;
				this.speed = speed;
				this.duration = duration;
			}

			public TransitionEvent()
			{
			}

		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// Provides a shader property that is set in the inspector
		/// and a material instantiated from the shader
		public Shader shader;
		[Range(0, 1.0f)] public float maskValue;
		public Color maskColor = Color.black;
		public Texture2D maskTexture;
		public bool maskInvert;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The current material being used by the shader
		/// </summary>
		private Material material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = new Material(this.shader)
					{
						hideFlags = HideFlags.HideAndDontSave
					};
				}
				return this.m_Material;
			}
		}
		private Material m_Material { get; set; }
		private bool m_maskInvert { get; set; }
		private StratusActionSet currentSeq { get; set; }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		/// <summary>
		/// Initializes the CameraTransition component.
		/// </summary>
		protected override void OnAwake()
		{
			StratusScene.Connect<TransitionEvent>(this.OnTransitionEvent);
		}

		private void OnDisable()
		{
			if (this.m_Material)
			{
				DestroyImmediate(this.m_Material);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.enabled)
			{
				Graphics.Blit(source, destination);
				return;
			}

			this.material.SetColor("_MaskColor", this.maskColor);
			this.material.SetFloat("_MaskValue", this.maskValue);
			this.material.SetTexture("_MainTex", source);
			this.material.SetTexture("_MaskTex", this.maskTexture);

			if (this.material.IsKeywordEnabled("INVERT_MASK") != this.maskInvert)
			{
				if (this.maskInvert)
				{
					this.material.EnableKeyword("INVERT_MASK");
				}
				else
				{
					this.material.DisableKeyword("INVERT_MASK");
				}
			}

			Graphics.Blit(source, destination, this.material);
		}

		private void OnTransitionEvent(TransitionEvent e)
		{
			// If there's a mask, change to it
			if (e.mask)
			{
				this.maskTexture = e.mask;
			}
			// Set the initial value instantly
			this.maskValue = e.initialValue;
			// Create a sequence for the transition
			this.currentSeq?.Cancel();
			this.currentSeq = StratusActions.Sequence(this);
			StratusActions.Property(this.currentSeq, () => this.maskValue, e.endingValue, e.speed, StratusEase.Linear);

			// If the transition is of a fixed duration
			if (e.duration > 0.0f)
			{
				StratusActions.Delay(this.currentSeq, e.duration);
				StratusActions.Property(this.currentSeq, () => this.maskValue, e.initialValue, e.speed, StratusEase.Linear);
			}
		}

		public static void Transition(TransitionEvent e)
		{
			StratusScene.Dispatch<StratusScreenShaderTransition.TransitionEvent>(e);
		}

	}

}