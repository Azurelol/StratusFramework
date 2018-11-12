using System;
using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Provides an automatic layout 2D Handle GUI besides default access to the SceneView GUI
	/// </summary>
	public abstract class LayoutSceneViewDisplay : SceneViewDisplay
	{
		/// <summary>
		/// A required attribute for configuring the LayoutSceneViewDisplay
		/// </summary>
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
		public sealed class LayoutViewDisplayAttributeAttribute : Attribute
		{
			public LayoutViewDisplayAttributeAttribute(string title, float width, float height, StratusGUI.Anchor anchor, StratusGUI.Dimensions dimensions)
			{
				this.title = title;
				this.width = width;
				this.height = height;
				this.anchor = anchor;
				this.dimensions = dimensions;
			}

			private string title { get; set; }
			private float width { get; set; }
			private float height { get; set; }
			private StratusGUI.Anchor anchor { get; set; }
			private StratusGUI.Dimensions dimensions { get; set; }
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public string title;
		public Vector2 offset;
		public StratusGUI.Anchor anchor;
		public StratusGUI.Dimensions dimensions;
		public Vector2 size;
		public Vector2 scale;
		private Vector2 scrollPos = Vector2.zero;
		private Rect rect;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public Vector2 currentSize { get; private set; }
		private bool hasCalculatedDimensions { get; set; }
		private int controlId { get; set; }

		//------------------------------------------------------------------------/
		// Methods: Interface
		//------------------------------------------------------------------------/
		protected abstract void OnGUILayout(Rect position);
		protected abstract void OnGUI(Rect position);

		//------------------------------------------------------------------------/
		// Methods: Override
		//------------------------------------------------------------------------/
		protected override void OnInitializeDisplay()
		{


		}

		/// <summary>
		/// Invoked by the Unity's Scene window. This is where we hook in our own drawing code
		/// </summary>
		/// <param name="sceneView"></param>
		protected override void OnSceneGUI(SceneView sceneView)
		{
			//if (!hasCalculatedDimensions && dimensions == StratusGUI.Dimensions.Relative)
			//{
			//  size = StratusGUI.FindRelativeDimensions(scale, sceneView.position.size);
			//  hasCalculatedDimensions = true;
			//}

			// What size to use
			this.currentSize = this.CalculateSize(sceneView);

			// Draw the default GUI
			this.OnGUI(sceneView.position);

			// Draw the provided layout 2D GUI Block
			//Draw_Old(sceneView);
			this.controlId = GUIUtility.GetControlID(FocusType.Keyboard);
			Rect lastRect = GUILayout.Window(this.controlId, this.rect, this.Draw, this.title, GUI.skin.window);
			this.rect = new Rect(lastRect.position, this.currentSize);
			//Draw(sceneView);
		}

		//------------------------------------------------------------------------/
		// Methods: Private
		//------------------------------------------------------------------------/
		private void Draw(int id)
		{
			Handles.BeginGUI();
			{
				this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, false, false);
				this.OnGUILayout(this.rect);
				GUILayout.EndScrollView();

			}
			Handles.EndGUI();
			GUI.DragWindow();
		}

		private void Draw_Old(SceneView sceneView)
		{
			Rect layoutPosition = StratusGUI.CalculateAnchoredPositionOnScreen(this.anchor, this.currentSize, sceneView.position.size);
			Handles.BeginGUI();
			{
				GUILayout.BeginArea(layoutPosition, this.title, GUI.skin.window);
				{
					this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, false, false);
					this.OnGUILayout(layoutPosition);
					GUILayout.EndScrollView();
				}
				GUILayout.EndArea();
			}
			Handles.EndGUI();
		}

		protected override void OnReset()
		{
			Type type = this.GetType();
			LayoutViewDisplayAttributeAttribute settings = AttributeUtility.FindAttribute<LayoutViewDisplayAttributeAttribute>(type);
			if (settings == null)
			{
				throw new MissingReferenceException("Missing [LayoutSceneViewDisplay] attribute declaration for the class '" + type.Name + "'");
			}

			SceneView sceneView = SceneView.currentDrawingSceneView;
			if (sceneView == null)
			{
				StratusDebug.Log("No scene view?");
				return;
			}

			// Read and set the properties fron the attribute
			this.title = settings.GetProperty<string>("title");
			this.size = new Vector2(settings.GetProperty<float>("width"), settings.GetProperty<float>("height"));
			this.anchor = settings.GetProperty<StratusGUI.Anchor>("anchor");
			this.dimensions = settings.GetProperty<StratusGUI.Dimensions>("dimensions");
			// If the size is relative...
			if (this.dimensions == StratusGUI.Dimensions.Relative)
			{
				this.scale = this.size;
			}
			// Calcualte the rect

			this.currentSize = this.CalculateSize(sceneView);
			this.rect = StratusGUI.CalculateAnchoredPositionOnScreen(this.anchor, this.currentSize, sceneView.position.size);
		}

		protected override void OnInspect()
		{
			this.anchor = (StratusGUI.Anchor)EditorGUILayout.EnumPopup("Anchor", this.anchor);
			this.dimensions = (StratusGUI.Dimensions)EditorGUILayout.EnumPopup("Dimensions", this.dimensions);
			switch (this.dimensions)
			{
				case StratusGUI.Dimensions.Relative:
					EditorGUI.indentLevel++;
					this.scale.x = EditorGUILayout.Slider("Horizontal", this.scale.x, 0f, 1f);
					this.scale.y = EditorGUILayout.Slider("Vertical", this.scale.y, 0f, 1f);
					EditorGUI.indentLevel--;
					//scale = EditorGUILayout.Slider("Scale", scale);
					break;
				case StratusGUI.Dimensions.Absolute:
					EditorGUI.indentLevel++;
					this.size.x = EditorGUILayout.FloatField("Width", this.size.x);
					this.size.y = EditorGUILayout.FloatField("Height", this.size.y);
					EditorGUI.indentLevel--;
					//size = EditorGUILayout.Vector2Field("Size", size);
					break;
			}
		}

		private Vector2 CalculateSize(SceneView sceneView)
		{
			Vector2 size = new Vector2();
			switch (this.dimensions)
			{
				case StratusGUI.Dimensions.Relative:
					size = StratusGUI.CalculateRelativeDimensions(this.scale, sceneView.position.size);
					break;
				case StratusGUI.Dimensions.Absolute:
					size = this.size;
					break;
			}
			return size;
		}



	}
}
