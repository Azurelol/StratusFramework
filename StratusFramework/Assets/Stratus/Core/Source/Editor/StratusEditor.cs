using System;
using System.Collections.Generic;
using System.Reflection;
using Rotorz.ReorderableList;
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
			public System.Action<Rect> onDraw;
			public Func<bool> onValidate;

			public DrawGroupRequest(System.Action<Rect> drawFunction, Func<bool> validateFunction = null)
			{
				this.onDraw = drawFunction;
				this.onValidate = validateFunction;
			}

			public bool isValid => this.onValidate != null ? this.onValidate() : true;
		}





		public class ReorderableListImplementation : Malee.Editor.ReorderableList
		{
			public ReorderableListImplementation(SerializedProperty list) : base(list)
			{
			}

			public ReorderableListImplementation(SerializedProperty list, bool canAdd, bool canRemove, bool draggable) : base(list, canAdd, canRemove, draggable)
			{
			}

			public ReorderableListImplementation(SerializedProperty list, bool canAdd, bool canRemove, bool draggable, ElementDisplayType elementDisplayType, string elementNameProperty, Texture elementIcon) : base(list, canAdd, canRemove, draggable, elementDisplayType, elementNameProperty, elementIcon)
			{
			}

			public ReorderableListImplementation(SerializedProperty list, bool canAdd, bool canRemove, bool draggable, ElementDisplayType elementDisplayType, string elementNameProperty, string elementNameOverride, Texture elementIcon) : base(list, canAdd, canRemove, draggable, elementDisplayType, elementNameProperty, elementNameOverride, elementIcon)
			{
			}
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Specific constraints that dictate whether a specified property should be displayed
		/// </summary>
		public PropertyConstraintMap propertyConstraints { get; set; } = new PropertyConstraintMap();
		/// <summary>
		/// Provided callbacks for when specific properties are changed
		/// </summary>
		public Dictionary<SerializedProperty, System.Action> propertyChangeCallbacks { get; set; } = new Dictionary<SerializedProperty, System.Action>();
		/// <summary>
		/// Overrides for drawing specific properties from specific types
		/// </summary>
		public Dictionary<SerializedProperty, System.Action<SerializedProperty>> propertyDrawOverrides { get; set; } = new Dictionary<SerializedProperty, Action<SerializedProperty>>();
		/// <summary>
		/// A map of all available properties by name
		/// </summary>
		public Dictionary<string, SerializedProperty> propertyMap { get; set; } = new Dictionary<string, SerializedProperty>();
		/// <summary>
		/// The custom attributes for this property
		/// </summary>
		public Dictionary<SerializedProperty, Attribute[]> propertyAttributes { get; private set; } = new Dictionary<SerializedProperty, Attribute[]>();
		/// <summary>
		/// The custom attributes for this property
		/// </summary>
		public Dictionary<SerializedProperty, Dictionary<Type, Attribute>> propertyAttributesMap { get; private set; } = new Dictionary<SerializedProperty, Dictionary<Type, Attribute>>();
		/// <summary>
		/// The set of properties of the most-derived class
		/// </summary>
		public Tuple<Type, StratusSerializedPropertyModel[]> declaredProperties => this.propertyGroups.Last();
		/// <summary>
		/// A map of all property groups by the type
		/// </summary>
		public Dictionary<Type, StratusSerializedPropertyModel[]> propertiesByType { get; set; } = new Dictionary<Type, StratusSerializedPropertyModel[]>();
		/// <summary>
		/// A map of all property groups by the type
		/// </summary>
		private Dictionary<Type, SerializedProperty[]> unityPropertiesByType { get; set; } = new Dictionary<Type, SerializedProperty[]>();
		/// <summary>
		/// A list of all different property groups, starting from the base class to the most-derived class
		/// </summary>
		public List<Tuple<Type, StratusSerializedPropertyModel[]>> propertyGroups { get; set; } = new List<Tuple<Type, StratusSerializedPropertyModel[]>>();
		/// <summary>
		/// A list of properties serialized by Odin, starting from the base class to the most derived class
		/// </summary>
		public List<Tuple<Type, StratusOdinSerializedProperty[]>> odinSerializedProperties { get; set; } = new List<Tuple<Type, StratusOdinSerializedProperty[]>>();
		/// <summary>
		/// Whether to draw labels for types above property groups
		/// </summary>
		public virtual bool drawTypeLabels => false;
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
		/// Whether any custom GUI styles have been configured
		/// </summary>
		private bool doneFirstUpdate { get; set; }
		/// <summary>
		/// Whethe the default styles are being overwritten
		/// </summary>
		public bool overrideStyles { get; set; } = false;
		/// <summary>
		/// The current GUI event in Unity
		/// </summary>
		protected UnityEngine.Event currentEvent => UnityEngine.Event.current;
		/// <summary>
		/// Whether to use reorderable lists for drawing arrays and lists
		/// </summary>
		public bool drawReorderableLists { get; set; } = true;
		/// <summary>
		/// A collection of all registered lists to be drawn with reoderable within this editor
		/// </summary>
		protected Dictionary<SerializedProperty, ReorderableListImplementation> reorderableLists { get; set; } = new Dictionary<SerializedProperty, ReorderableListImplementation>();
		/// <summary>
		/// A collection of all registered lists to be drawn with reoderable within this editor
		/// </summary>
		protected Dictionary<StratusOdinSerializedProperty, StratusReorderableList> stratusReorderableLists { get; set; } = new Dictionary<StratusOdinSerializedProperty, StratusReorderableList>();
		/// <summary>
		/// Any requests to be processed at the end of this frame
		/// </summary>
		protected List<System.Action> endOfFrameRequests { get; private set; } = new List<System.Action>();
		/// <summary>
		/// A list of all messages added
		/// </summary>
		public List<ObjectValidation> messages { get; private set; } = new List<ObjectValidation>();
		//public System.Action onEnteredEditMode { get; protected set; }
		//public System.Action onExitingEditMode { get; protected set; }
		//public System.Action onEnteredPlayMode { get; protected set; }
		//public System.Action onExitingdPlayMode { get; protected set; }
		/// <summary>
		/// Whether there are messages to be shown
		/// </summary>
		public bool hasMessages => this.messages.NotEmpty();
		/// <summary>
		/// The base type for the component type this editor is for. It marks the stopping point to look at properties.
		/// </summary>
		protected abstract Type baseType { get; }
		/// <summary>
		/// The number of properties drawn by the base editor GUI
		/// </summary>
		protected int drawnProperties { get; set; }
		/// <summary>
		/// Whether the target component is still valid
		/// </summary>
		protected bool isTargetValid => !this.target.IsDestroyed();



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

			this.InitializeBaseEditor();
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
			//switch (stateChange)
			//{
			//  case PlayModeStateChange.EnteredEditMode:
			//    onEnteredEditMode?.Invoke();
			//    break;
			//
			//  case PlayModeStateChange.ExitingEditMode:
			//    onExitingEditMode?.Invoke();
			//    break;
			//
			//  case PlayModeStateChange.EnteredPlayMode:
			//    onEnteredPlayMode?.Invoke();
			//    break;
			//
			//  case PlayModeStateChange.ExitingPlayMode:
			//    onExitingdPlayMode?.Invoke();
			//    break;
			//}
		}

		public void InitializeBaseEditor()
		{
			this.AddProperties();
			this.OnStratusGenericEditorEnable();
			this.OnStratusEditorEnable();
		}

		public override void OnInspectorGUI()
		{
			//StratusGUIStyles.OverrideDefaultFont();

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

			//StratusGUIStyles.RevertDefaultFont();
		}

		public virtual void OnBaseEditorGUI()
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
					EditorGUILayout.LabelField(properties.Item1.Name, StratusGUIStyles.headerWhite);
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
					EditorGUILayout.LabelField(this.declaredProperties.Item1.Name, StratusGUIStyles.headerWhite);
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

		public virtual void DrawMessages()
		{
			foreach (ObjectValidation message in this.messages)
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

		private void RemoveMessage(ObjectValidation message)
		{
			this.endOfFrameRequests.Add(() => { this.messages.Remove(message); });
		}

		//------------------------------------------------------------------------/
		// Helpers
		//------------------------------------------------------------------------/
		/// <summary>
		/// Gets all the serialized property for the given Unity Object of a specified type
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Tuple<SerializedProperty[], StratusOdinSerializedProperty[]> GetSerializedProperties(SerializedObject serializedObject, Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

			List<SerializedProperty> serializedProperties = new List<SerializedProperty>();
			List<StratusOdinSerializedProperty> odinSerializedProperties = new List<StratusOdinSerializedProperty>();

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo field = fields[i];
				if (field != null && (field.Attributes != FieldAttributes.NotSerialized))
				{
					bool serializedbyOdin = OdinSerializer.UnitySerializationUtility.OdinWillSerialize(field, true);
					bool serializedByUnity = OdinSerializer.UnitySerializationUtility.GuessIfUnityWillSerialize(field);

					// Odin
					if (serializedbyOdin && !serializedByUnity)
					{
						StratusOdinSerializedProperty property = new StratusOdinSerializedProperty(field, serializedObject.targetObject);
						odinSerializedProperties.Add(property);
					}
					// Unity
					else
					{
						SerializedProperty property = serializedObject.FindProperty(field.Name);
						if (property != null)
						{
							serializedProperties.Add(property);
						}
					}
				}
			}

			return new Tuple<SerializedProperty[], StratusOdinSerializedProperty[]>(serializedProperties.ToArray(), odinSerializedProperties.ToArray());
		}

		public static StratusSerializedPropertyModel[] GetSerializedPropertyDrawers(SerializedObject serializedObject, Type type)
		{
			Tuple<SerializedProperty[], StratusOdinSerializedProperty[]> properties = GetSerializedProperties(serializedObject, type);
			List<StratusSerializedPropertyModel> serializedPropertyDrawRequests = new List<StratusSerializedPropertyModel>();
			foreach (SerializedProperty property in properties.Item1)
			{
				serializedPropertyDrawRequests.Add(new StratusSerializedPropertyModel(property));
			}

			foreach (StratusOdinSerializedProperty property in properties.Item2)
			{
				serializedPropertyDrawRequests.Add(new StratusSerializedPropertyModel(property));
			}

			return serializedPropertyDrawRequests.ToArray();
		}

		public static StratusSerializedPropertyModel[] GetSerializedPropertyDrawers(SerializedProperty[] serializedProperties, StratusOdinSerializedProperty[] odinSerializedProperties)
		{
			List<StratusSerializedPropertyModel> serializedPropertyDrawRequests = new List<StratusSerializedPropertyModel>();
			foreach (SerializedProperty property in serializedProperties)
			{
				serializedPropertyDrawRequests.Add(new StratusSerializedPropertyModel(property));
			}

			foreach (StratusOdinSerializedProperty property in odinSerializedProperties)
			{
				serializedPropertyDrawRequests.Add(new StratusSerializedPropertyModel(property));
			}

			return serializedPropertyDrawRequests.ToArray();
		}

		public static SerializedProperty[] GetSerializedProperties(SerializedObject serializedObject)
		{
			List<SerializedProperty> properties = new List<SerializedProperty>();
			//Trace.Script($"Properties for {serializedObject.targetObject.GetType().Name}");
			SerializedProperty iter = serializedObject.GetIterator();
			if (iter != null)
			{
				// THe first property is m_Script
				iter.NextVisible(true);
				//properties.Add(iter);
				//Trace.Script($"- {iter.name}");

				while (iter.NextVisible(false))
				{
					properties.Add(iter);
					//Trace.Script($"- {iter.name}");
				}
			}
			iter.Reset();
			return properties.ToArray();
		}

		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>
		/// <returns>True if the property changed, false otherwise.</returns>

		public bool DrawSerializedProperty(SerializedProperty property, SerializedObject serializedObject)
		{
			EditorGUI.BeginChangeCheck();

			// If this property has been requested to be drawn in a custom manner
			if (this.propertyDrawOverrides.ContainsKey(property))
			{
				this.propertyDrawOverrides[property].Invoke(property);
			}
			else
			{
				// Arrays
				if (property.isArray && property.propertyType != SerializedPropertyType.String && this.drawReorderableLists)
				{
					this.DrawReorderableList(property, property.displayName);
				}
				// Use normal drawers
				else
				{
					bool overridden = false;
					if (!overridden)
					{
						// Custom enum drawer, ho!
						if (property.propertyType == SerializedPropertyType.Enum)
						{
							StratusSearchableEnum.EnumPopup(property);
						}
						else
						{
							EditorGUILayout.PropertyField(property, true);
						}
					}
				}
			}

			// If property was changed, save
			if (EditorGUI.EndChangeCheck())
			{
				// Record change
				Undo.RecordObject(this.target, property.name);

				// Apply the modified property
				serializedObject.ApplyModifiedProperties();

				// Inform that this property has been changed
				if (this.propertyChangeCallbacks.ContainsKey(property))
				{
					this.propertyChangeCallbacks[property].Invoke();
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Draws a property (really, a field) serialized by Odin Serializer
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public bool DrawSerializedProperty(StratusOdinSerializedProperty property)
		{
			bool changed = false;
			EditorGUI.BeginChangeCheck();
			{
				if (property.isArray)
				{
					this.DrawReorderableList(property);
				}
				else
				{
					changed |= StratusEditorUtility.DrawField(property.field, this.target);
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(this.target);
				// Record change
				Undo.RecordObject(this.target, property.field.Name);
				// Apply the modified property
				this.serializedObject.ApplyModifiedProperties();
				changed = true;
			}
			return changed;
		}

		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public bool DrawSerializedProperty(SerializedProperty property)
		{
			return this.DrawSerializedProperty(property, this.serializedObject);
		}

		/// <summary>
		/// Returns the serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public SerializedProperty GetSerializedProperty(string propertyName)
		{
			return this.propertyMap[propertyName];
		}

		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="prop"></param>
		/// <returns>True if the property changed, false if it was not drawn or found.</returns>
		public bool DrawSerializedProperty(string propertyName)
		{
			if (!this.propertyMap.ContainsKey(propertyName))
			{
				return false;
			}

			return this.DrawSerializedProperty(this.propertyMap[propertyName], this.serializedObject);
		}

		/// <summary>
		/// Adds a reorderable list drawer to the editor
		/// </summary>
		/// <param name="property"></param>
		private void AddReorderableList(SerializedProperty property)
		{
			this.reorderableLists.Add(property, new ReorderableListImplementation(property));
		}

		/// <summary>
		/// Adds a reorderable list drawer to the editor
		/// </summary>
		/// <param name="property"></param>
		private void AddReorderableList(StratusOdinSerializedProperty property)
		{
			this.stratusReorderableLists.Add(property, StratusReorderableList.PolymorphicList(property));
		}

		/// <summary>
		/// Draws the given array type using a reorderable list drawer
		/// </summary>
		/// <param name="property"></param>
		/// <param name="label"></param>
		protected void DrawReorderableList(SerializedProperty property, string label)
		{
			//reorderableLists[property].DoLayoutList();
			ReorderableListGUI.Title(label);
			ReorderableListGUI.ListField(property);
		}

		/// <summary>
		/// Draws the given array type using a reorderable list drawer
		/// </summary>
		/// <param name="property"></param>
		/// <param name="label"></param>
		protected void DrawReorderableList(StratusOdinSerializedProperty property)
		{
			this.stratusReorderableLists[property].DoLayoutList();
		}

		/// <summary>
		/// Draws the given array type using a reorderable list drawer
		/// </summary>
		/// <param name="property"></param>
		/// <param name="label"></param>
		public static void DrawReorderableList<T>(List<T> list, string label, ReorderableListControl.ItemDrawer<T> drawItem)
		{
			ReorderableListGUI.Title(label);
			ReorderableListGUI.ListField(list, drawItem);
		}

		/// <summary>
		/// Draws the given array type using a reorderable list drawer
		/// </summary>
		/// <param name="property"></param>
		/// <param name="label"></param>
		public static void DrawReorderableList<T>(List<T> list, ReorderableListControl.ItemDrawer<T> drawItem)
		{
			ReorderableListGUI.ListField(list, drawItem);
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void DoFirstUpdate()
		{
			if (this.backgroundStyle == null)
			{
				this.backgroundStyle = StratusGUIStyles.background;
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

		/// <summary>
		/// Draws the given set of properties according to any present constraints in the editor
		/// </summary>
		/// <param name="properties"></param>
		private bool DrawSerializedProperties(StratusSerializedPropertyModel[] properties)
		{
			bool propertiesChanged = false;
			foreach (StratusSerializedPropertyModel property in properties)
			{
				switch (property.type)
				{
					case StratusSerializedPropertyModel.SerializationType.Unity:
						bool hasConstraint = this.propertyConstraints.ContainsKey(property.unitySerialized);
						if (hasConstraint)
						{
							bool canBeDrawn = this.propertyConstraints[property.unitySerialized].Invoke();
							if (!canBeDrawn)
							{
								continue;
							}
						}
						propertiesChanged |= this.DrawSerializedProperty(property.unitySerialized, this.serializedObject);
						break;

					case StratusSerializedPropertyModel.SerializationType.Odin:
						propertiesChanged |= this.DrawSerializedProperty(property.odinSerialized);
						break;
				}


			}
			return propertiesChanged;
		}

		/// <summary>
		/// Adds all SerializedProperties to be inspected
		/// </summary>
		internal void AddProperties()
		{
			// Reset the number of drawn properties
			this.drawnProperties = 0;

			// For every type, starting from the most derived up to the base, get its serialized properties      
			Type declaredType = this.target.GetType();
			Type currentType = declaredType;
			Type previousType = null;
			//SerializedProperty[] properties = null;

			while (currentType != this.baseType)
			{
				//Trace.Script($"Adding properties for {currentType.Name}");

				// Serialized Properties
				Tuple<SerializedProperty[], StratusOdinSerializedProperty[]> propertiesSplit = GetSerializedProperties(this.serializedObject, currentType);
				StratusSerializedPropertyModel[] unityProperties = GetSerializedPropertyDrawers(propertiesSplit.Item1, propertiesSplit.Item2);

				foreach (StratusSerializedPropertyModel property in unityProperties)
				{
					// Record this property
					if (property == null)
					{
						Debug.LogError($"A property was found to not be serialized properly while inspecting {this.target.name}. Did you forget a [Serializable] attribute on a class definition?");
						//continue;
					}

					if (property.type != StratusSerializedPropertyModel.SerializationType.Unity)
					{
						continue;
					}

					SerializedProperty serializedProperty = property.unitySerialized;

					//serializedProperty.tar
					// Map the property
					this.propertyMap.Add(serializedProperty.name, serializedProperty);

					// Record the attributes for this property
					Attribute[] attributes = serializedProperty.GetFieldAttributes();
					this.propertyAttributes.Add(serializedProperty, attributes);
					this.propertyAttributesMap.AddIfMissing(serializedProperty, new Dictionary<Type, Attribute>());
					foreach (Attribute attr in attributes)
					{
						this.propertyAttributesMap[serializedProperty].AddIfMissing(attr.GetType(), attr);
					}
					this.OnPropertyAttributesAdded(serializedProperty);

					// Check whether this property is an array
					if (serializedProperty.isArray && serializedProperty.propertyType != SerializedPropertyType.String)
					{
						this.AddReorderableList(serializedProperty);
					}
				}

				foreach (StratusOdinSerializedProperty odinProperty in propertiesSplit.Item2)
				{
					if (odinProperty.isArray)
					{
						this.AddReorderableList(odinProperty);
					}
				}

				// Add all the properties for this type into the property map by type        
				if (!currentType.IsGenericType)
				{
					this.propertiesByType.Add(currentType, unityProperties);
					this.unityPropertiesByType.Add(currentType, propertiesSplit.Item1);
					this.propertyGroups.Add(new Tuple<Type, StratusSerializedPropertyModel[]>(currentType, unityProperties));
				}
				else
				{
					// Unity
					SerializedProperty[] joinedUnityProperties = this.unityPropertiesByType[previousType].Concat(propertiesSplit.Item1);
					this.unityPropertiesByType[previousType] = joinedUnityProperties;
					// Combined
					StratusSerializedPropertyModel[] joinedProperties = this.propertiesByType[previousType].Concat(unityProperties);
					this.propertiesByType[previousType] = joinedProperties;
					// Concat property groups
					this.propertyGroups.RemoveLast();
					this.propertyGroups.Add(new Tuple<Type, StratusSerializedPropertyModel[]>(previousType, joinedProperties));
				}

				// Move on to the parent type (if any)
				previousType = currentType;
				currentType = currentType.BaseType;
			}


			this.propertyGroups.Reverse();
		}

		public SerializedProperty[] GetSerializedPropertiesOfType(Type type)
		{
			return this.unityPropertiesByType[type];
		}

		/// <summary>
		/// Checks whether all the properties are under constraints. Returns false if none
		/// of the properties can be drawn.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		private bool ValidateConstraints(StratusSerializedPropertyModel[] properties)
		{
			foreach (StratusSerializedPropertyModel property in properties)
			{
				if (property.type != StratusSerializedPropertyModel.SerializationType.Unity)
				{
					continue;
				}

				bool foundConstraint = this.propertyConstraints.ContainsKey(property.unitySerialized);

				// If no constraint was found for this property, it means 
				// that at least one property can be drawn
				if (!foundConstraint)
				{
					return true;
				}
				// If the property was found and validated, it means we can draw it
				else
				{
					bool validated = this.propertyConstraints[property.unitySerialized]();
					if (validated)
					{
						return true;
					}
				}
			}

			// No constraints were validated
			return false;
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
		/// Always returns true
		/// </summary>
		/// <returns></returns>
		protected bool True()
		{
			return true;
		}

		/// <summary>
		/// Always returns false
		/// </summary>
		protected bool False()
		{
			return false;
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

		///// <summary>
		///// Adds a constraint that decides whether a given property is drawn
		///// </summary>
		///// <param name="propertyName"></param>
		///// <param name="constraint"></param>
		//protected void AddConstraint(string propertyName, System.Func<bool> constraint)
		//{
		//  propertyConstraints.Add(propertyMap[propertyName], constraint);
		//}

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
				this.propertyConstraints.Add(this.propertyMap[propertyName], constraint);
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
			ObjectValidation contextMessage = new ObjectValidation(message, type.Convert(), target);
			this.messages.Add(contextMessage);
		}

		/// <summary>
		/// Adds a validation message on top of the editor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="target"></param>
		public void AddMessage(ObjectValidation message)
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

	}

	/// <summary>
	/// Base editor for all Stratus components
	/// </summary>
	public abstract class StratusBehaviourEditor<T> : StratusEditor where T : MonoBehaviour
	{
		protected new T target { get; private set; }
		protected override Type baseType => typeof(MonoBehaviour);

		protected virtual void OnBehaviourEditorValidate() { }

		internal override void OnStratusGenericEditorEnable()
		{
			this.SetTarget();
		}

		internal override void OnGenericStratusEditorValidate()
		{
			if (!this.target)
			{
				this.SetTarget();
			}

			if (this.target)
			{
				this.OnBehaviourEditorValidate();
			}
		}

		private void SetTarget()
		{
			this.target = base.target as T;
		}

	}

	/// <summary>
	/// Base editor for all Stratus components
	/// </summary>
	public abstract class ScriptableEditor<T> : StratusEditor where T : ScriptableObject
	{
		public override bool drawTypeLabels { get; } = true;

		protected new T target { get; private set; }
		protected override Type baseType => typeof(ScriptableObject);

		protected virtual void OnScriptableEditorValidate() { }

		internal override void OnStratusGenericEditorEnable()
		{
			this.target = base.target as T;
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