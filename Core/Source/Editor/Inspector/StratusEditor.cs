using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Base editor for all Stratus components.
	/// </summary>
	public abstract partial class StratusEditor : UnityEditor.Editor
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// Maps serialized properties to validating functions
		/// </summary>
		/// 
		public class PropertyConstraintMap : Dictionary<SerializedProperty, Func<bool>> { }

		/// <summary>
		/// Used to encapsulate the drawing of a particular group
		/// </summary>
		public class DrawGroupRequest
		{
			public string label;
			public Action<Rect> onDraw;
			public Func<bool> onValidate;

			public DrawGroupRequest(System.Action<Rect> drawFunction, Func<bool> validateFunction = null)
			{
				this.onDraw = drawFunction;
				this.onValidate = validateFunction;
			}

			public bool isValid => this.onValidate != null ? this.onValidate() : true;
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/	
		/// <summary>
		/// Whether to draw labels for types above property groups
		/// </summary>
		public virtual bool drawTypeLabels { get; set; } = false;
		/// <summary>
		/// Custom draw functions to be invoked after property drawing
		/// </summary>
		protected List<DrawGroupRequest> drawGroupRequests { get; set; } = new List<DrawGroupRequest>();
		/// <summary>
		/// The default label style for headers
		/// </summary>
		public GUIStyle labelStyle { get; set; }
		/// <summary>
		/// The default background style used
		/// </summary>
		public GUIStyle backgroundStyle { get; set; }
		/// <summary>
		/// The default style used for each section
		/// </summary>
		public GUIStyle sectionStyle { get; set; }
		/// <summary>
		/// The current GUI event in Unity
		/// </summary>
		protected Event currentEvent => UnityEngine.Event.current;
		/// <summary>
		/// A collection of all registered lists to be drawn with reoderable within this editor
		/// </summary>
		protected Dictionary<StratusSerializedPropertyModel, StratusReorderableList> reorderableLists { get; set; } = new Dictionary<StratusSerializedPropertyModel, StratusReorderableList>();
		/// <summary>
		/// Any requests to be processed at the end of this frame
		/// </summary>
		protected List<Action> endOfFrameRequests { get; private set; } = new List<System.Action>();
		/// <summary>
		/// A list of all messages added
		/// </summary>
		public List<StratusObjectValidation> messages { get; private set; } = new List<StratusObjectValidation>();
		/// <summary>
		/// Whether there are messages to be shown
		/// </summary>
		public bool hasMessages => this.messages.NotEmpty();
		/// <summary>
		/// The base type for the component type this editor is for. It marks the stopping point to look at properties.
		/// </summary>
		protected abstract Type baseType { get; }
		/// <summary>
		/// The type of the object being inspected
		/// </summary>
		protected Type targetType { get; private set; }
		/// <summary>
		/// Whether the target component is still valid
		/// </summary>
		protected bool isTargetValid => !this.target.IsNull();

		//------------------------------------------------------------------------/
		// Properties: Overrides
		//------------------------------------------------------------------------/	
		/// <summary>
		/// Whethe the default styles are being overwritten
		/// </summary>
		protected virtual bool overrideStyles => false;
		/// <summary>
		/// Whether to override the default font for the inspector
		/// </summary>
		protected virtual bool overrideFont => false;
		/// <summary>
		/// Whether to use reorderable lists for drawing arrays and lists
		/// </summary>
		protected virtual bool drawReorderableLists => false;
		/// <summary>
		/// Whether to draw enum fields using popup drawers
		/// </summary>
		protected virtual bool drawEnumPopup => true;

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/	
		private bool doneFirstUpdate;
		private Dictionary<SerializedProperty, StratusSerializedPropertyModel> unitySerializedPropertyModels { get; set; } = new Dictionary<SerializedProperty, StratusSerializedPropertyModel>();
		private Dictionary<StratusSerializedField, StratusSerializedPropertyModel> customSerializedPropertyModels { get; set; } = new Dictionary<StratusSerializedField, StratusSerializedPropertyModel>();

		//------------------------------------------------------------------------/
		// Virtual Methods
		//------------------------------------------------------------------------/
		protected abstract void OnStratusEditorEnable();
		protected virtual void OnStratusEditorDisable() { }
		protected virtual void OnFirstUpdate() { }
		internal abstract void OnStratusGenericEditorEnable();
		internal abstract void OnGenericStratusEditorValidate();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void OnEnable()
		{
			if (this.target == null)
			{
				return;
			}

			this.Initialize();
			EditorApplication.playModeStateChanged += this.OnPlayModeStateChanged;
		}

		private void OnDisable()
		{
			EditorApplication.playModeStateChanged -= this.OnPlayModeStateChanged;

			if (this.isTargetValid)
			{
				this.OnStratusEditorDisable();
			}
		}

		protected virtual void OnPlayModeStateChanged(PlayModeStateChange stateChange)
		{
		}

		public override void OnInspectorGUI()
		{
			StratusGUIStyles.OverrideDefaultFont();

			// Invoke the very first time
			if (!this.doneFirstUpdate)
			{
				this.DoFirstUpdate();
			}

			// Now fulfill any custom requests at the end of inspection
			this.ProcessEndOfFrameRequests();

			// Update the serialized object, saving data
			this.serializedObject.Update();

			// Show any messages, if present
			if (this.messages.NotEmpty())
			{
				this.DrawMessages();
			}

			// Now draw the base editor
			if (this.drawnProperties > 0)
			{
				this.OnBaseEditorGUI();
			}

			// Now draw invokable methods
			if (this.buttons.NotEmpty())
			{
				this.DrawButtons();
			}

			// Now draw any custom draw functions
			if (this.drawGroupRequests.Count > 0)
			{
				foreach (DrawGroupRequest drawRequest in this.drawGroupRequests)
				{
					if (drawRequest.isValid)
					{
						Rect rect = EditorGUILayout.BeginVertical(this.backgroundStyle);
						drawRequest.onDraw(rect);
						EditorGUILayout.EndVertical();
					}
				}
			}
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void Initialize()
		{
			this.targetType = this.target.GetType();
			this.ScanProperties();
			this.ScanMethods();
			this.OnStratusGenericEditorEnable();
			this.OnStratusEditorEnable();
		}

		protected virtual void OnBaseEditorGUI()
		{
			bool propertiesChanged = false;

			// Reverse order: Draw all the types up until the most-derived
			for (int i = 0; i < this.propertyGroups.Count - 1; i++)
			{
				Tuple<Type, StratusSerializedPropertyModel[]> properties = this.propertyGroups[i];

				// If there's no properties for this type
				if (properties.Item2.Length < 1)
				{
					continue;
				}

				// If all properties fail the constraints check
				if (!this.ValidateConstraints(properties.Item2))
				{
					continue;
				}

				if (this.drawTypeLabels)
				{
					EditorGUILayout.LabelField(properties.Item1.GetNiceName(), StratusGUIStyles.headerWhite);
				}

				EditorGUILayout.BeginVertical(this.backgroundStyle);
				propertiesChanged |= this.DrawSerializedProperties(properties.Item2);
				EditorGUILayout.EndVertical();
			}

			// Now draw the declared properties
			if (this.declaredProperties.Item2.Length > 0)
			{
				if (this.drawTypeLabels)
				{
					EditorGUILayout.LabelField(this.declaredProperties.Item1.GetNiceName(), StratusGUIStyles.headerWhite);
				}

				EditorGUILayout.BeginVertical(this.backgroundStyle);
				propertiesChanged |= this.DrawDeclaredProperties();
				EditorGUILayout.EndVertical();
			}

			// If any properties changed, inform!
			if (propertiesChanged && !(this.target == null || this.target.Equals(null)))
			{
				this.OnGenericStratusEditorValidate();
			}
		}

		protected virtual void DrawButtons()
		{
			this.DrawVerticalArea(() =>
			{
				foreach (StratusLabeledContextAction<StratusInvokeMethodAttribute> button in this.buttons)
				{
					GUI.enabled = button.context.isPlayMode == EditorApplication.isPlaying;
					if (GUILayout.Button(button.label, EditorStyles.miniButton))
					{
						button.action();
					}
					GUI.enabled = true;
				}
			});
		}

		public virtual void DrawMessages()
		{
			foreach (StratusObjectValidation message in this.messages)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox(message.message, message.type.Convert());
				if (message.hasContext)
				{
					StratusEditorUtility.OnMouseClick(GUILayoutUtility.GetLastRect(), null, () =>
					{
						GenericMenu menu = new GenericMenu();

						if (message.onSelect != null)
						{
							menu.AddItem(new GUIContent("Select"), false, () => { message.onSelect(); });
						}

						if (message.onValidate == null)
						{
							menu.AddItem(new GUIContent("Remove"), false, () => this.RemoveMessage(message));
						}

						menu.ShowAsContext();
					});
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		private void ProcessEndOfFrameRequests()
		{
			if (this.endOfFrameRequests.NotEmpty())
			{
				foreach (Action request in this.endOfFrameRequests)
				{
					request.Invoke();
				}

				this.endOfFrameRequests.Clear();
			}
		}

		private void DrawVerticalArea(System.Action gui)
		{
			EditorGUILayout.BeginVertical(this.backgroundStyle);
			gui();
			EditorGUILayout.EndVertical();
		}

		private void RemoveMessage(StratusObjectValidation message)
		{
			this.endOfFrameRequests.Add(() => { this.messages.Remove(message); });
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void DoFirstUpdate()
		{
			if (this.backgroundStyle == null)
			{
				this.backgroundStyle = EditorStyles.helpBox;
			}

			if (this.labelStyle == null)
			{
				this.labelStyle = StratusGUIStyles.skin.label;
			}

			this.OnFirstUpdate();
			this.doneFirstUpdate = true;
		}

		protected virtual bool DrawDeclaredProperties()
		{
			return this.DrawSerializedProperties(this.declaredProperties.Item2);
		}

		protected void DrawEditor(UnityEditor.Editor editor, string header, int headerSize = 12)
		{
			EditorGUILayout.Space();
			EditorGUILayout.InspectorTitlebar(false, editor.target, false);

			EditorGUI.indentLevel = 1;
			editor.OnInspectorGUI();
			EditorGUI.indentLevel = 0;
		}

		/// <summary>
		/// Adds a new section to the editor
		/// </summary>
		/// <param name="drawFunction"></param>
		/// <param name="validateFunction"></param>
		protected void AddArea(System.Action<Rect> drawFunction, Func<bool> validateFunction = null)
		{
			this.drawGroupRequests.Add(new DrawGroupRequest(drawFunction, validateFunction));
		}
		
		/// <summary>
		/// Adds a constraint that decides whether a given property is drawn
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="constraint"></param>
		protected void AddConstraint(System.Func<bool> constraint, params SerializedProperty[] properties)
		{
			foreach (SerializedProperty property in properties)
			{
				this.propertyConstraints.Add(property, constraint);
			}
		}

		/// <summary>
		/// Adds a constraint that decides whether a given group of properties is drawn
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="constraint"></param>
		protected void AddConstraint(System.Func<bool> constraint, params string[] propertyNames)
		{
			foreach (string propertyName in propertyNames)
			{
				this.propertyConstraints.AddUnique(this.propertyMap[propertyName], constraint);
			}
		}

		/// <summary>
		/// Adds a constraint that decides whether a given property is drawn
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="constraint"></param>
		protected void RemoveConstraint(string propertyName)
		{
			this.propertyConstraints.Remove(this.propertyMap[propertyName]);
		}

		protected void RecordModification(string description = null)
		{
			this.serializedObject.UpdateIfRequiredOrScript();
			this.serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Adds a constraint that decides whether a given group of properties is drawn
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="constraint"></param>
		protected void RemoveConstraints(params string[] propertyNames)
		{
			foreach (string propertyName in propertyNames)
			{
				this.propertyConstraints.Remove(this.propertyMap[propertyName]);
			}
		}

		/// <summary>
		/// Adds a validation message on top of the editor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="target"></param>
		public void AddMessage(string message, MessageType type, UnityEngine.Object target, Func<bool> onValidate = null)
		{
			StratusObjectValidation contextMessage = new StratusObjectValidation(message, type.Convert(), target);
			this.messages.Add(contextMessage);
		}

		/// <summary>
		/// Adds a validation message on top of the editor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="target"></param>
		public void AddMessage(StratusObjectValidation message)
		{
			this.messages.Add(message);
		}

		/// <summary>
		/// Hides a property from being drawn by default
		/// </summary>
		/// <param name="propertyName"></param>
		public void HideProperty(string propertyName)
		{
			this.AddConstraint(this.False, propertyName);
		}

		/// <summary>
		/// When the specified property is changed, invokes the given function
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="onChanged"></param>
		public void AddPropertyChangeCallback(string propertyName, System.Action onChanged)
		{
			SerializedProperty property = this.propertyMap[propertyName];
			this.AddPropertyChangeCallback(property, onChanged);
		}

		/// <summary>
		/// When the specified property is changed, invokes the given function
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="onChanged"></param>
		public void AddPropertyChangeCallback(SerializedProperty property, System.Action onChanged)
		{
			this.propertyChangeCallbacks.Add(property, onChanged);
		}

		/// <summary>
		/// Always returns true
		/// </summary>
		/// <returns></returns>
		protected bool True() => true;

		/// <summary>
		/// Always returns false
		/// </summary>
		protected bool False() => false;
	}

	

	/// <summary>
	/// Base editor for all Stratus components
	/// </summary>
	public abstract class StratusScriptableEditor<T> : StratusEditor where T : ScriptableObject
	{
		public override bool drawTypeLabels => false;
		protected new T target { get; private set; }
		protected override Type baseType => typeof(ScriptableObject);

		protected virtual void OnScriptableEditorValidate() { }

		internal override void OnStratusGenericEditorEnable()
		{
			this.target = base.target as T;
			drawTypeLabels = true;
		}

		internal override void OnGenericStratusEditorValidate()
		{
			if (this.target)
			{
				this.OnScriptableEditorValidate();
			}
		}

	}

}