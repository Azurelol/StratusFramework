using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Rotorz.ReorderableList;
using UnityEditorInternal;

namespace Stratus
{
  /// <summary>
  /// Base editor for all Stratus components.
  /// </summary>
  public abstract class BaseEditor : Editor
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    /// <summary>
    /// Maps serialized properties to validating functions
    /// </summary>
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

      public bool isValid => onValidate != null ? onValidate() : true;
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
    public Dictionary<Type, System.Action> drawTypeOverrides { get; set; } = new Dictionary<Type, System.Action>();
    /// <summary>
    /// A map of all available properties by name
    /// </summary>
    public Dictionary<string, SerializedProperty> propertyMap { get; set; } = new Dictionary<string, SerializedProperty>();
    /// <summary>
    /// The set of properties of the most-derived class
    /// </summary>
    public Tuple<Type, SerializedProperty[]> declaredProperties => propertyGroups.Last();
    /// <summary>
    /// A map of all property groups by the type
    /// </summary>
    public Dictionary<Type, SerializedProperty[]> propertiesByType { get; set; } = new Dictionary<Type, SerializedProperty[]>();
    /// <summary>
    /// A list of all different property groups, starting from the base class to the most-derived class
    /// </summary>
    public List<Tuple<Type, SerializedProperty[]>> propertyGroups { get; set; } = new List<Tuple<Type, SerializedProperty[]>>();
    /// <summary>
    /// Whether to draw labels for types above property groups
    /// </summary>
    public virtual bool drawTypeLabels => false;
    /// <summary>
    /// A collection of all registered lists to be drawn with reoderable within this editor
    /// </summary>
    protected Dictionary<SerializedProperty, ReorderableList> reorderableLists { get; set; } = new Dictionary<SerializedProperty, ReorderableList>();
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
    /// Any requests to be processed at the end of this frame
    /// </summary>
    protected List<System.Action> endOfFrameRequests { get; private set; } = new List<System.Action>();

    //------------------------------------------------------------------------/
    // Virtual Methods
    //------------------------------------------------------------------------/
    protected virtual void OnBaseEditorEnable() {}
    protected virtual void OnFirstUpdate() {}
    internal abstract void OnGenericBaseEditorEnable();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnEnable()
    {
      if (target == null)
      {
        return;
      }
      
      InitializeBaseEditor();
    }

    public void InitializeBaseEditor()
    {
      AddProperties();
      OnGenericBaseEditorEnable();
      OnBaseEditorEnable();
    }

    public override void OnInspectorGUI()
    {
      // Invoke the very first time
      if (!doneFirstUpdate)
      {
        if (backgroundStyle == null)
          backgroundStyle = StratusGUIStyles.box;
        if (labelStyle == null)
          labelStyle = StratusGUIStyles.skin.label;

        OnFirstUpdate();
        doneFirstUpdate = true;
      }

      // Update the serialized object, saving data
      serializedObject.Update();

      // Now draw the base editor
      OnBaseEditorGUI();

      // Now draw any custom draw functions
      if (drawGroupRequests.Count > 0)
      {
        foreach(var drawRequest in drawGroupRequests)
        {
          if (drawRequest.isValid)
          {
            Rect rect = EditorGUILayout.BeginVertical(backgroundStyle);
            drawRequest.onDraw(rect);
            EditorGUILayout.EndVertical();
          }
        }
      }

      // Now fulfill any custom requests at the end of inspection
      if (endOfFrameRequests.NotEmpty())
      {
        foreach(var request in endOfFrameRequests)
        {
          request.Invoke();
        }
        endOfFrameRequests.Clear();
      }
    }

    public virtual void OnBaseEditorGUI()
    {
      // Reverse order: Draw all the types up until the most-derived
      for (int i = 0; i < propertyGroups.Count - 1; i++)
      {
        var properties = propertyGroups[i];

        // If there's no properties for this type
        if (properties.Item2.Length < 1)
          continue;

        // If all properties fail the constraints check
        if (!ValidateConstraints(properties.Item2))
          continue;


        if (drawTypeLabels)
          EditorGUILayout.LabelField(properties.Item1.Name, labelStyle);

        EditorGUILayout.BeginVertical(backgroundStyle);
        DrawProperties(properties.Item2);
        EditorGUILayout.EndVertical();
      }

      // Now draw the declared properties
      if (declaredProperties.Item2.Length > 0)
      {
        if (drawTypeLabels)
          EditorGUILayout.LabelField(declaredProperties.Item1.Name, labelStyle);
        EditorGUILayout.BeginVertical(backgroundStyle);
        DrawDeclaredProperties();
        //DrawProperties(declaredProperties.Item2);
        EditorGUILayout.EndVertical();
      }
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
    public static SerializedProperty[] GetSerializedProperties(SerializedObject serializedObject, Type type)
    {
      FieldInfo[] propInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
      SerializedProperty[] properties = new SerializedProperty[propInfo.Length];

      for (int a = 0; a < properties.Length; a++)
      {
        properties[a] = serializedObject.FindProperty(propInfo[a].Name);

        if (properties[a] == null)
        {
          //Trace.Script("Could not find property: " + propInfo[a].Name);
        }
      }

      return properties;
    }

    /// <summary>
    /// Draws a serialized property, saving any recorded changes
    /// </summary>
    /// <param name="property"></param>
    /// <returns>True if the property changed, false otherwise.</returns>

    public bool DrawSerializedProperty(SerializedProperty property, SerializedObject serializedObject)
    {
      EditorGUI.BeginChangeCheck();

      // Arrays
      if (property.isArray && property.propertyType != SerializedPropertyType.String && drawReorderableLists)
      {
        DrawReorderableList(property, property.displayName);
      }
      else
      {
        EditorGUILayout.PropertyField(property, true);
      }

      // If property was changed, save
      if (EditorGUI.EndChangeCheck())
      {
        // Record change
        Undo.RecordObject(target, property.name);

        // Apply the modified property
        serializedObject.ApplyModifiedProperties();

        // Inform that this property has been changed
        if (propertyChangeCallbacks.ContainsKey(property))
          propertyChangeCallbacks[property].Invoke();
        return true;
      }

      return false;
    }

    /// <summary>
    /// Draws a serialized property, saving any recorded changes
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public bool DrawSerializedProperty(SerializedProperty prop)
    {
      return DrawSerializedProperty(prop, serializedObject);
    }

    /// <summary>
    /// Draws a serialized property, saving any recorded changes
    /// </summary>
    /// <param name="prop"></param>
    /// <returns>True if the property changed, false if it was not drawn or found.</returns>
    public bool DrawSerializedProperty(string propertyName)
    {
      if (!propertyMap.ContainsKey(propertyName))
        return false;

      return DrawSerializedProperty(propertyMap[propertyName], serializedObject);
    }

    /// <summary>
    /// Draws the given array type using a reorderable list drawer
    /// </summary>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public static void DrawReorderableList(SerializedProperty property, string label)
    {
      ReorderableListGUI.Title(label);
      ReorderableListGUI.ListField(property);
    }

    public static ReorderableList GetListWithFoldout(SerializedObject serializedObject, SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
    {
      var list = new ReorderableList(serializedObject, property, draggable, displayHeader, displayAddButton, displayRemoveButton);

      list.drawHeaderCallback = (Rect rect) =>
      {
        var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
        property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, property.displayName);
      };
      list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
          if (!property.isExpanded)
          {
            GUI.enabled = index == list.count;
            return;
          }

          var element = list.serializedProperty.GetArrayElementAtIndex(index);
          rect.y += 2;
          EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
      list.elementHeightCallback = (int indexer) =>
      {
        if (!property.isExpanded)
          return 0;
        else
          return list.elementHeight;
      };

      return list;
    }

    //------------------------------------------------------------------------/
    // Procedures
    //------------------------------------------------------------------------/
    protected virtual void DrawDeclaredProperties()
    {
      DrawProperties(declaredProperties.Item2);
    }

    /// <summary>
    /// Draws the given set of properties according to any present constraints in the editor
    /// </summary>
    /// <param name="properties"></param>
    private void DrawProperties(SerializedProperty[] properties)
    {
      foreach (var prop in properties)
      {
        bool hasConstraint = propertyConstraints.ContainsKey(prop);
        if (hasConstraint)
        {
          bool canBeDrawn = propertyConstraints[prop].Invoke();
          if (!canBeDrawn)
            continue;
        }

        DrawSerializedProperty(prop, serializedObject);
      }
    }

    internal void AddProperties()
    {
      // For every type, starting from the most derived up to the base, get its serialized properties      
      Type declaredType = target.GetType();
      Type currentType = declaredType;
      while (currentType != typeof(MonoBehaviour))
      {
        //Trace.Script($"Adding properties for {currentType.Name}");

        // Add the properties onto the map
        var properties = GetSerializedProperties(serializedObject, currentType);
        foreach (var prop in properties)
        {
          // Check the attributes for this proeprty
          //prop.
          //Trace.Script($"- {prop.name}");

          propertyMap.Add(prop.name, prop);
          if (prop.isArray && prop.propertyType != SerializedPropertyType.String)
          {
            ReorderableList list = GetListWithFoldout(serializedObject, prop, true, true, true, true);
            reorderableLists.Add(prop, list);
          }
        }

        // Add all the properties for this type into the property map by type        
        propertiesByType.Add(currentType, properties);
        propertyGroups.Add(new Tuple<Type, SerializedProperty[]>(currentType, properties));

        currentType = currentType.BaseType;
      }

      propertyGroups.Reverse();
    }



    /// <summary>
    /// Checks whether all the properties are under constraints. Returns false if none
    /// of the properties can be drawn.
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    private bool ValidateConstraints(SerializedProperty[] properties)
    {
      foreach(var prop in properties)
      {
        bool foundConstraint = propertyConstraints.ContainsKey(prop);

        // If no constraint was found for this property, it means 
        // that at least one property can be drawn
        if (!foundConstraint)
          return true;
        // If the property was found and validated, it means we can draw it
        else
        {
          bool validated = propertyConstraints[prop]();
          if (validated)
            return true;
        }        
      }

      // No constraints were validated
      return false;
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

    /// <summary>
    /// Checks whether the mouse was over the last GUI control
    /// </summary>
    /// <returns></returns>
    protected bool IsMouseOverLastControl()
    {
      Rect buttonRect = GUILayoutUtility.GetLastRect();
      return buttonRect.Contains(currentEvent.mousePosition);
    }
    //
    ///// <summary>
    ///// Creates a custom base editor for the selected class
    ///// </summary>
    ///// <param name="targetObject"></param>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public new static BaseEditor CreateEditor(UnityEngine.Object targetObject)
    //{
    //  BaseEditor editor = Editor.CreateEditor(targetObject) as BaseEditor;
    //  //editor.InitializeBaseEditor();
    //  return editor;
    //}

  }

  /// <summary>
  /// Base editor for all Stratus components
  /// </summary>
  public abstract class BaseEditor<T> : BaseEditor where T : MonoBehaviour
  {
    protected new T target { get; private set; }

    internal override void OnGenericBaseEditorEnable()
    {
      target = base.target as T;
    }

    //private void OnEnable()
    //{
    //  target = base.target as T;
    //
    //  if (target == null)
    //    return;
    //
    //  AddProperties();
    //  OnBaseEditorEnable();
    //}



  }

}