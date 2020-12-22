using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
  /// <summary>
  /// Handles drawing a serialized property: keeping track of property and decorator drawers
  /// </summary>
  public class StratusSerializedPropertyHandler
  {
    public const float PROPERTY_MARGIN = 2f;

    private PropertyDrawer propertyDrawer = null;
    public string tooltip = null;

    private List<DecoratorDrawer> decoratorDrawers = null;
    public List<ContextMenuItemAttribute> contextMenuItems = null;

    public bool HasPropertyDrawer
    {
      get
      {
        return PropertyDrawer != null;
      }
    }

    private PropertyDrawer PropertyDrawer
    {
      get
      {
        return (!IsCurrentlyNested) ? propertyDrawer : null;
      }
    }

    private bool IsCurrentlyNested
    {
      get
      {
        return propertyDrawer != null && StratusScriptAttributeUtility.s_DrawerStack.Any() && propertyDrawer == StratusScriptAttributeUtility.s_DrawerStack.Peek();
      }
    }

    public bool Empty
    {
      get
      {
        return decoratorDrawers == null && tooltip == null && PropertyDrawer == null && contextMenuItems == null;
      }
    }

    /// <summary>
    /// Extract attribute data for property
    /// </summary>
    public void HandleAttribute(PropertyAttribute attribute, FieldInfo field, Type propertyType)
    {
      if (attribute is TooltipAttribute)
      {
        tooltip = (attribute as TooltipAttribute).tooltip;
      }
      else if (attribute is ContextMenuItemAttribute)
      {
        if (!propertyType.IsArrayOrList())
        {
          if (contextMenuItems == null)
          {
            contextMenuItems = new List<ContextMenuItemAttribute>();
          }
          contextMenuItems.Add(attribute as ContextMenuItemAttribute);
        }
      }
      else
      {
        HandleDrawnType(attribute.GetType(), propertyType, field, attribute);
      }
    }

    /// <summary>
    /// Extract Property and Decorator drawers for property
    /// </summary>
    public void HandleDrawnType(Type drawnType, Type propertyType, FieldInfo field, PropertyAttribute attribute)
    {
      Type drawerTypeForType = StratusScriptAttributeUtility.GetDrawerTypeForType(drawnType);
      if (drawerTypeForType != null)
      {
        if (typeof(PropertyDrawer).IsAssignableFrom(drawerTypeForType))
        {
          if (propertyType != null && propertyType.IsArrayOrList())
          {
            return;
          }
          propertyDrawer = (PropertyDrawer)Activator.CreateInstance(drawerTypeForType);

          propertyDrawer.SetFieldInfo(field);
          propertyDrawer.SetAttribute(attribute);
        }
        else if (typeof(DecoratorDrawer).IsAssignableFrom(drawerTypeForType))
        {
          if (field != null && field.FieldType.IsArrayOrList() && !propertyType.IsArrayOrList())
          {
            return;
          }
          DecoratorDrawer decoratorDrawer = (DecoratorDrawer)Activator.CreateInstance(drawerTypeForType);

          decoratorDrawer.SetAttribute(attribute);
          if (decoratorDrawers == null)
          {
            decoratorDrawers = new List<DecoratorDrawer>();
          }
          decoratorDrawers.Add(decoratorDrawer);
        }
      }
    }

    /// <summary>
    /// Handle the Drawing of a given property without the need of a position Rect.
    /// </summary>
    public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
    {
      bool haveDecorators = (decoratorDrawers != null && decoratorDrawers.Count != 0);
      bool needCustomRect = (property.propertyType != SerializedPropertyType.Boolean || PropertyDrawer != null || haveDecorators);

      Rect curRect;
      if (needCustomRect)
      {
        bool hasLabel = StratusEditorUtility.LabelHasContent(label);
        float height = GetHeight(property, label, includeChildren);
        curRect = EditorGUILayout.GetControlRect(hasLabel, height, options);
      }
      else
      {
        curRect = StratusEditorUtility.GetToggleRect(true, options);
      }

      StratusEditorUtility.lastEditorGUILayoutRect = curRect;
      return OnGUI(curRect, property, label, includeChildren);
    }

    /// <summary>
    /// Handle the Drawing of a given property, within a given position rect.
    /// </summary>
    public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
    {
      // Keep track of the height we have available to draw this property after decorator drawers
      float heighRemaining = position.height;
      position.height = 0f;

      // Handle Decorator Drawers for non-nested properties
      if (decoratorDrawers != null && !IsCurrentlyNested)
      {
        foreach (DecoratorDrawer decoratorDrawer in decoratorDrawers)
        {
          position.height = decoratorDrawer.GetHeight();

          float labelWidth = EditorGUIUtility.labelWidth;
          float fieldWidth = EditorGUIUtility.fieldWidth;

          // Draw the decorator with its default OnGUI
          decoratorDrawer.OnGUI(position);

          EditorGUIUtility.labelWidth = labelWidth;
          EditorGUIUtility.fieldWidth = fieldWidth;

          position.y += position.height;
          heighRemaining -= position.height;
        }
      }

      position.height = heighRemaining;

      // Handle Property drawers
      if (PropertyDrawer != null)
      {
        float labelWidth = EditorGUIUtility.labelWidth;
        float fieldWidth = EditorGUIUtility.fieldWidth;

        GUIContent curLabel = label ?? StratusEditorUtility.TempContent(property.displayName);
        PropertyDrawer.OnGUISafe(position, property.Copy(), curLabel);

        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUIUtility.fieldWidth = fieldWidth;
        return false;
      }

      position.height = StratusEditorUtility.GetSinglePropertyHeight(property, label);
      bool isVisible = StratusEditorUtility.DefaultPropertyField(position, property, label);
      position.y += position.height;

      // No children - we're done
      if (!includeChildren)
        return isVisible;

      // Handle Children
      if (isVisible && StratusEditorUtility.HasVisibleChildFields(property))
      {
        Vector2 localIconSize = EditorGUIUtility.GetIconSize();
        bool localEnabled = GUI.enabled;
        int localIndentLevel = EditorGUI.indentLevel;

        SerializedProperty begin = null;
        SerializedProperty end = null;

        // Array/List: Get paginated begin/end, draw header, and update position to where paginated elements should be drawn
        if (property.isArray)
        {
          //Redacted.PaginatedArrayUtilities.GetPagePropertyRange(property, ref begin, ref end);

          //SerializedProperty sizeProperty = property.Copy();
          //sizeProperty.NextVisible(true);

          //EditorGUI.BeginChangeCheck();
          //Rect headerRect = Redacted.PaginatedArrayUtilities.HandleArrayHeader(position, sizeProperty);
          //if (EditorGUI.EndChangeCheck())
          //  return false;
          //
          //position.y += headerRect.height;
          //position.height -= headerRect.height;
        }
        // Default: Just get first and last child
        else
        {
          begin = property.Copy();
          begin.NextVisible(true);
          end = property.GetEndProperty();
        }

        // Draw all nested properties from begin to end
        if (begin != null && end != null)
          position = DrawPropertyNestedRange(position, EditorGUI.indentLevel - property.depth, begin, end);

        EditorGUIUtility.SetIconSize(localIconSize);
        GUI.enabled = localEnabled;
        EditorGUI.indentLevel = localIndentLevel;
      }

      return false;
    }

    /// <summary>
    /// Handle the height calculation for the current property
    /// </summary>
    public float GetHeight(SerializedProperty inputProperty, GUIContent label, bool includeChildren)
    {
      float heightSoFar = 0f;

      // Handle decorator drawer heights
      if (decoratorDrawers != null && !IsCurrentlyNested)
      {
        // add up the heigh of all attached decorators
        foreach (DecoratorDrawer decoratorDrawer in decoratorDrawers)
        {
          heightSoFar += decoratorDrawer.GetHeight();
        }
      }

      // Copy the property to ensure we don't break caller's position
      SerializedProperty property = inputProperty.Copy();

      // If this is drawn by a Property drawer, use its height
      if (PropertyDrawer != null)
      {
        GUIContent curLabel = label ?? StratusEditorUtility.TempContent(property.displayName);
        return PropertyDrawer.GetPropertyHeightSafe(property, curLabel) + heightSoFar;
      }

      // If this is an array header, special case it
      //if (Redacted.PaginatedArrayUtilities.IsArraySizeProperty(property))
      //{
      //  return Redacted.PaginatedArrayUtilities.GetArrayHeaderSize(property) + heightSoFar;
      //}

      // Start with the default drawer
      heightSoFar += StratusEditorUtility.GetSinglePropertyHeight(property, label);

      // Has children that need to be drawn
      if (includeChildren && property.isExpanded)
      {
        SerializedProperty begin = null;
        SerializedProperty end = null;

        // Special case for paginated arrays
        if (property.isArray)
        {
          SerializedProperty sizeProperty = property.Copy();
          sizeProperty.NextVisible(true);
          //heightSoFar += StratusEditorUtility.GetSinglePropertyHeight(sizeProperty, )
          //heightSoFar += Redacted.PaginatedArrayUtilities.GetArrayHeaderSize(sizeProperty);
          //Redacted.PaginatedArrayUtilities.GetPagePropertyRange(property, ref begin, ref end);
        }
        else
        {
          begin = property.Copy();
          begin.NextVisible(true);
          end = property.GetEndProperty();
        }

        if (begin != null && end != null)
          heightSoFar += GetHeightNestedRange(begin, end);
      }

      return heightSoFar;
    }

    /// <summary>
    /// Draw all properties from begin to end, and return the rect taken up.
    /// </summary>
    private Rect DrawPropertyNestedRange(Rect position, int indentOffset, SerializedProperty begin, SerializedProperty end)
    {
      // Track the top so we know how tall the whole drawn area was
      float top = position.yMin;

      SerializedProperty cur = begin.Copy();

      while (!SerializedProperty.EqualContents(cur, end))
      {
        EditorGUI.indentLevel = cur.depth + indentOffset;
        position.y += PROPERTY_MARGIN;

        StratusSerializedPropertyHandler curHandler = StratusScriptAttributeUtility.GetHandler(cur);
        position.height = curHandler.GetHeight(cur, StratusEditorUtility.TempContent(cur.displayName), cur.isExpanded);

        EditorGUI.BeginChangeCheck();
        curHandler.OnGUI(position, cur, null, true);
        position.y += position.height;

        if (EditorGUI.EndChangeCheck())
          break;

        cur.NextVisible(false);
      }

      position.yMin = top;
      return position;
    }

    /// <summary>
    /// Calculate the height taken up by all properties from begin to end.
    /// </summary>
    private float GetHeightNestedRange(SerializedProperty begin, SerializedProperty end)
    {
      float height = 0f;

      SerializedProperty cur = begin.Copy();

      while (!SerializedProperty.EqualContents(cur, end))
      {
        StratusSerializedPropertyHandler curHandler = StratusScriptAttributeUtility.GetHandler(cur);
        float curHeight = curHandler.GetHeight(cur, StratusEditorUtility.TempContent(cur.displayName), cur.isExpanded);
        height += curHeight + PROPERTY_MARGIN;
        cur.NextVisible(false);
      }

      return height;
    }
  }

}