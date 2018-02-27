using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System;

namespace Stratus
{
  [CustomEditor(typeof(TriggerSystem))]
  public class TriggerSystemEditor : BaseEditor<TriggerSystem>
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private GUIStyle selectedButtonStyle;
    private GUIStyle buttonStyle;
    private const float connectedButtonWidth = 20f;
    private List<Tuple<Trigger, Trigger>> triggerSwapOperations = new List<Tuple<Trigger, Trigger>>();
    private List<Tuple<Triggerable, Triggerable>> triggerableSwapOperations = new List<Tuple<Triggerable, Triggerable>>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    private BaseEditor selectedEditor { get; set; }
    private string selectedName { get; set; }
    private Triggerable selectedTriggerable { get; set; }
    private Trigger selectedTrigger { get; set; }
    private TriggerBase selected { get; set; }
    private Dictionary<Triggerable, bool> connectedTriggerables { get; set; } = new Dictionary<Triggerable, bool>();
    private Dictionary<Trigger, bool> connectedTriggers { get; set; } = new Dictionary<Trigger, bool>();
    private List<Trigger> triggers => target.triggers;
    private List<Triggerable> triggerables => target.triggerables;
    private bool showDescriptions => target.showDescriptions;
    private Vector2 scrollPos { get; set; } = Vector2.zero;
    private int maxColumnElements => Mathf.Max(target.triggers.Count, target.triggerables.Count);
    private float buttonHeight { get; set; } = 42f;
    private float currentColumnHeight => Mathf.Min(maxColumnElements * buttonHeight, 7 * buttonHeight);
    private TypeSelector triggerTypes, triggerableTypes;
    private GUILayoutOption addTriggerLabelWidth = GUILayout.Width(100f);
    private GUILayoutOption columnWidth;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnBaseEditorEnable()
    {
      triggerTypes = new TypeSelector(typeof(Trigger), true, true);
      triggerTypes.selectionHint = "(Select Trigger)";
      triggerableTypes = new TypeSelector(typeof(Triggerable), true, true);
      triggerableTypes.selectionHint = "(Select Triggerable)";

      selected = null;
      selectedTrigger = null;
      selectedTriggerable = null;
      selectedEditor = null;

      drawGroupRequests.Add(new DrawGroupRequest(DrawOptions));
      drawGroupRequests.Add(new DrawGroupRequest(AddNew));
      drawGroupRequests.Add(new DrawGroupRequest(DrawConnections, () => { return maxColumnElements != 0; }));
      drawGroupRequests.Add(new DrawGroupRequest(DrawSelected, () => { return selectedEditor != null; }));

      // For recompilation
      if (selectedTrigger || selectedTriggerable)
        UpdateConnections();
    }

    protected override void OnFirstUpdate()
    {
      buttonStyle = StratusGUIStyles.button;
      selectedButtonStyle = StratusGUIStyles.blueButton;
    }

    public override void OnBaseEditorGUI()
    {
    }

    //------------------------------------------------------------------------/
    // Methods: Drawing
    //------------------------------------------------------------------------/
    private void DrawConnections(Rect rect)
    {
      // Draw triggers and triggerables side by side
      //scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Height(currentColumnHeight), GUILayout.ExpandWidth(false));
      //Trace.Script($"rect = {rect}, currentViewWidth = {EditorGUIUtility.currentViewWidth}");
      columnWidth = GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f);
      //rect.width = rect.width / 2f;
      //GUI.Label(rect, "Triggers", StratusGUIStyles.header);
      //rect.x += rect.width;      
      //GUI.Label(rect, "Triggerables", StratusGUIStyles.header);

      GUILayout.BeginHorizontal();
      GUILayout.Label("Triggers", StratusGUIStyles.header);
      GUILayout.Label("Triggerables", StratusGUIStyles.header);
      GUILayout.EndHorizontal();
      
      GUILayout.BeginHorizontal();
      {
        GUILayout.BeginVertical();
        {
          //EditorGUILayout.LabelField("Triggers", EditorStyles.label);
          DrawTriggers();
        }
        GUILayout.EndVertical();
        

        GUILayout.BeginVertical();
        {
          //EditorGUILayout.LabelField("Triggerables", EditorStyles.la);
          DrawTriggerables();
        }
        GUILayout.EndVertical();
        
      }
      GUILayout.EndHorizontal();
      //GUI.EndGroup();
      //EditorGUILayout.EndScrollView();
    }

    private void DrawTriggers()
    {
      foreach (var trigger in target.triggers)
        DrawTrigger(trigger);

      // If any swap operations...
      if (!triggerSwapOperations.Empty())
      {
        foreach (var tuple in triggerSwapOperations)
          triggers.Swap(tuple.Item1, tuple.Item2);
        triggerSwapOperations.Clear();
      }


    }

    private void DrawTriggerables()
    {

      foreach (var triggerable in target.triggerables)
        DrawTriggerable(triggerable);

      // If any swap operations...
      if (!triggerableSwapOperations.Empty())
      {
        foreach (var tuple in triggerableSwapOperations)
          triggerables.Swap(tuple.Item1, tuple.Item2);
        triggerableSwapOperations.Clear();
      }
    }

    void DrawSelected(Rect rect)
    {
      if (selectedEditor == null)
        return;

      {
        EditorGUILayout.LabelField(selectedName, EditorStyles.whiteBoldLabel);
        EditorGUI.indentLevel = 1;
        selectedEditor.OnInspectorGUI();
        EditorGUI.indentLevel = 0;
      }
    }

    private void DrawOptions(Rect rect)
    {
      //EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
      DrawSerializedProperty(propertyMap["showDescriptions"], serializedObject);
    }

    private void DrawConnection(Triggerable triggerable)
    {
      if (!selectedTrigger)
      {
        //GUILayout.Button(string.Empty, StratusEditorStyles., GUILayout.Width(connectedButtonWidth));
        return;
      }

      bool isConnected = connectedTriggerables[triggerable];
      if (isConnected)
      {
        if (GUILayout.Button(string.Empty, StratusGUIStyles.blueCircleButton, GUILayout.Width(connectedButtonWidth)))
          Disconnect(selectedTrigger, triggerable);
      }
      else
      {
        if (GUILayout.Button(string.Empty, StratusGUIStyles.greyCircleButton, GUILayout.Width(connectedButtonWidth)))
          Connect(selectedTrigger, triggerable);
      }
    }

    private void DrawTrigger(Trigger trigger)
    {
      GUIStyle style = buttonStyle;
      if (selected)
      {
        if (selected == trigger)
          style = selectedButtonStyle;
        else if (selectedTriggerable && connectedTriggers.ContainsKey(trigger) && connectedTriggers[trigger])
          style = StratusGUIStyles.greenButton;
      }

      Draw(trigger, style, SelectTrigger, RemoveTrigger, SetTriggerContextMenu);
    }

    private void DrawTriggerable(Triggerable triggerable)
    {
      GUIStyle style = buttonStyle;
      if (selected)
      {
        if (selected == triggerable)
          style = selectedButtonStyle;
        else if (selectedTrigger && connectedTriggerables.ContainsKey(triggerable) && connectedTriggerables[triggerable])
          style = StratusGUIStyles.greenButton;
      }

      Draw(triggerable, style, SelectTriggerable, RemoveTriggerable, SetTriggerableContextMenu);
    }

    private void Draw<T>(T baseTrigger, GUIStyle style, System.Action<T> selectFunction, System.Action<T> removeFunction, System.Action<T, GenericMenu> contextMenuSetter) where T : TriggerBase
    {
      string name = baseTrigger.GetType().Name;

      System.Action onLeftClick = () =>
      {
        selectedName = name;
        selected = baseTrigger;
        selectFunction(baseTrigger);
        GUI.FocusControl(string.Empty);
        UpdateConnections();
      };

      System.Action onRightClick = () =>
      {
        var menu = new GenericMenu();
        contextMenuSetter(baseTrigger, menu);
        menu.AddSeparator("");
          // Enable
          menu.AddItem(new GUIContent(baseTrigger.enabled ? "Disable" : "Enable"), false, () =>
            {
          baseTrigger.enabled = !baseTrigger.enabled;
        });
          // Duplicate
          menu.AddItem(new GUIContent("Duplicate/New"), false, () =>
            {
          target.gameObject.DuplicateComponent(baseTrigger, false);
        });
        menu.AddItem(new GUIContent("Duplicate/Copy"), false, () =>
        {
          target.gameObject.DuplicateComponent(baseTrigger, true);
          UpdateConnections();
        });
          // Remove
          menu.AddItem(new GUIContent("Remove"), false, () =>
            {
          removeFunction(baseTrigger);
          UpdateConnections();
        });
        menu.ShowAsContext();
      };

      System.Action onDrag = () =>
      {
          //Drag(baseTrigger, name);
        };

      System.Action<object> onDrop = (object other) =>
      {
        if (baseTrigger is Trigger && other is Trigger)
        {
          triggerSwapOperations.Add(new Tuple<Trigger, Trigger>(baseTrigger as Trigger, other as Trigger));
        }
        else if (baseTrigger is Triggerable && other is Triggerable)
        {
          triggerableSwapOperations.Add(new Tuple<Triggerable, Triggerable>(baseTrigger as Triggerable, other as Triggerable));
        }
      };

      Func<object, bool> onValidateDrag = (object other) =>
      {
        if (other is Trigger || other is Triggerable)
          return true;
        return false;
      };

      var button = new GUIObject();
      button.label = baseTrigger.enabled ? name : $"<color=grey>{name}</color>";
      button.description = $"<i>{baseTrigger.description}</i>";
      button.showDescription = showDescriptions;
      button.descriptionsWithLabel = target.descriptionsWithLabel;
      button.onLeftClickUp = onLeftClick;
      button.onRightClickDown = onRightClick;
      button.onDrag = onDrag;
      button.onDrop = onDrop;
      button.dragDataIdentifier = "Stratus Trigger System Button";
      button.dragData = baseTrigger;
      button.onValidateDrag = onValidateDrag;
      button.Draw(style, columnWidth);
    }

    private void SetTriggerContextMenu(Trigger trigger, GenericMenu menu)
    {
      // Connect
      if (selectedTriggerable)
      {
        if (IsConnected(trigger, selectedTriggerable))
          menu.AddItem(new GUIContent($"Disconnect from {selectedTriggerable.GetType().Name}"), false, () => Disconnect(trigger, selectedTriggerable));
        else
          menu.AddItem(new GUIContent($"Connect to {selectedTriggerable.GetType().Name}"), false, () => Connect(trigger, selectedTriggerable));

        menu.AddSeparator("");
      }

      menu.AddItem(new GUIContent("Disconnnect Triggerables"), false, () =>
      {
        trigger.targets.Clear();
        UpdateConnections();
      });

    }

    private void SetTriggerableContextMenu(Triggerable triggerable, GenericMenu menu)
    {
      // Connect
      if (selectedTrigger)
      {
        if (connectedTriggerables[triggerable])
          menu.AddItem(new GUIContent($"Disconnect from {selectedTrigger.GetType().Name}"), false, () => Disconnect(selectedTrigger, triggerable));
        else
          menu.AddItem(new GUIContent($"Connect to {selectedTrigger.GetType().Name}"), false, () => Connect(selectedTrigger, triggerable));
      }

    }

    //------------------------------------------------------------------------/
    // Methods: Selection
    //------------------------------------------------------------------------/
    private void SelectTrigger(Trigger trigger)
    {
      selectedTriggerable = null;
      selectedTrigger = trigger;

      // Instantiate the editor for it, disable drawwing base trigger properties
      selectedEditor = Editor.CreateEditor(trigger) as BaseEditor;
      selectedEditor.backgroundStyle = EditorStyles.helpBox;

      var baseTriggerProperties = selectedEditor.propertiesByType[typeof(Trigger)];
      foreach (var property in baseTriggerProperties)
      {
        // We still want to select persistence
        if (property.name == "persistent")
          continue;

        selectedEditor.propertyConstraints.Add(property, False);
      }
    }

    private void SelectTriggerable(Triggerable triggerable)
    {
      selectedTrigger = null;
      selectedTriggerable = triggerable;

      selectedEditor = Editor.CreateEditor(triggerable) as BaseEditor;
      selectedEditor.backgroundStyle = EditorStyles.helpBox;
    }

    private void Deselect()
    {
      selected = null;
      selectedEditor = null;
    }

    private void RemoveTrigger(Trigger trigger)
    {
      if (selected == trigger)
      {
        Deselect();
        selectedTrigger = null;
      }
      target.triggers.Remove(trigger);
      Undo.DestroyObjectImmediate(trigger);
    }

    private void RemoveTriggerable(Triggerable triggerable)
    {
      if (selected == triggerable)
      {
        Deselect();
        selectedTriggerable = null;
      }
      target.triggerables.Remove(triggerable);
      Undo.DestroyObjectImmediate(triggerable);
    }

    private void AddNew(Rect rect)
    {
      //GUIStyle buttonStyle = EditorStyles.miniButtonRight;
      // Select a trigger type
      EditorGUILayout.BeginHorizontal();
      {
        EditorGUILayout.LabelField("Add Trigger", EditorStyles.whiteLabel, addTriggerLabelWidth);
        bool changed = triggerTypes.GUILayoutPopup();
        if (changed)
        {
          target.gameObject.AddComponent(triggerTypes.selectedClass);
          triggerTypes.ResetSelection(0);
        }
        //if (GUILayout.Button("Add", buttonStyle))
      }
      EditorGUILayout.EndHorizontal();

      // Select a triggerable type
      EditorGUILayout.BeginHorizontal();
      {
        EditorGUILayout.LabelField("Add Triggerable", EditorStyles.whiteLabel, addTriggerLabelWidth);
        bool changed = triggerableTypes.GUILayoutPopup();
        if (changed)
        {
          target.gameObject.AddComponent(triggerableTypes.selectedClass);
          triggerableTypes.ResetSelection(0);
        }
        //if (GUILayout.Button("Add", buttonStyle))
      }
      EditorGUILayout.EndHorizontal();
    }

    //------------------------------------------------------------------------/
    // Methods: Connections
    //------------------------------------------------------------------------/
    private void Connect(Trigger trigger, Triggerable triggerable)
    {
      Trace.Script($"Connecting {trigger.GetType().Name} and {triggerable.GetType().Name}");
      trigger.targets.Add(triggerable);
      UpdateConnections();
      //connectedTriggerables[triggerable] = true;
      //connectedTriggers[trigger] = true;
    }

    private void Disconnect(Trigger trigger, Triggerable triggerable)
    {
      Trace.Script($"Disconnecting {trigger.GetType().Name} and {triggerable.GetType().Name}");
      trigger.targets.Remove(triggerable);
      UpdateConnections();
      //connectedTriggerables[triggerable] = false;
      //connectedTriggers[trigger] = false;
    }

    private bool IsConnected(Trigger trigger, Triggerable triggerable)
    {
      if (trigger.targets.Contains(triggerable))
        return true;
      return false;
    }

    private void UpdateConnections()
    {
      connectedTriggerables.Clear();
      if (selectedTrigger)
      {
        foreach (var triggerable in target.triggerables)
          connectedTriggerables.Add(triggerable, IsConnected(selectedTrigger, triggerable));
      }

      connectedTriggers.Clear();
      if (selectedTriggerable)
      {
        foreach (var trigger in target.triggers)
          connectedTriggers.Add(trigger, IsConnected(trigger, selectedTriggerable));
      }
    }


  }

}