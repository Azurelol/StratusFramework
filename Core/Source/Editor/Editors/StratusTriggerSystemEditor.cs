using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusTriggerSystem))]
  public partial class TriggerSystemEditor : StratusBehaviourEditor<StratusTriggerSystem>
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    //private TriggerSystemEditorGraph graph;
    //private XNodeEditor.NodeGraphEditor graphEditor;
    //private TriggerSystemCanvas canvas;
    private Color selectedColor;
    private Color connectedColor;
    private Color disconnectedColor;
    private GUIStyle buttonStyle;
    private const int descriptionSize = 10;
    private const float connectedButtonWidth = 20f;
    private List<Tuple<StratusTriggerBehaviour, StratusTriggerBehaviour>> triggerSwapOperations = new List<Tuple<StratusTriggerBehaviour, StratusTriggerBehaviour>>();
    private List<Tuple<StratusTriggerable, StratusTriggerable>> triggerableSwapOperations = new List<Tuple<StratusTriggerable, StratusTriggerable>>();
    private GUILayoutOption columnWidth;
    private StratusTypeSelector triggerTypes, triggerableTypes;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    private StratusEditor selectedEditor { get; set; }
    private string selectedName { get; set; }
    private StratusTriggerable selectedTriggerable { get; set; }
    private StratusTriggerBehaviour selectedTrigger { get; set; }
    private StratusTriggerBase selected { get; set; }
    private Dictionary<StratusTriggerable, bool> connectedTriggerables { get; set; } = new Dictionary<StratusTriggerable, bool>();
    private Dictionary<StratusTriggerBehaviour, bool> connectedTriggers { get; set; } = new Dictionary<StratusTriggerBehaviour, bool>();
    public Dictionary<StratusTriggerable, StratusTriggerSystem.ConnectionStatus> triggerableConnectivity { get; private set; } = new Dictionary<StratusTriggerable, StratusTriggerSystem.ConnectionStatus>();
    public List<StratusTriggerBehaviour> triggers => target.triggers;
    public List<StratusTriggerable> triggerables => target.triggerables;
    public Dictionary<StratusTriggerBase, int> connectivityGroups { get; set; } = new Dictionary<StratusTriggerBase, int>();
    private bool showDescriptions => target.showDescriptions;
    private Vector2 scrollPos { get; set; } = Vector2.zero;
    private int maxColumnElements => Mathf.Max(target.triggers.Count, target.triggerables.Count);
    private float buttonHeight { get; set; } = 42f;
    private float currentColumnHeight => Mathf.Min(maxColumnElements * buttonHeight, 7 * buttonHeight);    
    //private bool showMessages { get; set; }    

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnStratusEditorEnable()
    {
      triggerTypes = new StratusTypeSelector(typeof(StratusTriggerBehaviour), true);
      triggerableTypes = new StratusTypeSelector(typeof(StratusTriggerable), true);

      selectedColor = StratusGUIStyles.Colors.selectedColor;
      connectedColor = StratusGUIStyles.Colors.connectedColor;
      disconnectedColor = StratusGUIStyles.Colors.disconnectedColor;

      selected = null;
      selectedTrigger = null;
      selectedTriggerable = null;
      selectedEditor = null;

      //drawGroupRequests.Add(new DrawGroupRequest(DrawOptions));
      //drawGroupRequests.Add(new DrawGroupRequest(AddNew));
      drawGroupRequests.Add(new DrawGroupRequest(DrawConnections));
      drawGroupRequests.Add(new DrawGroupRequest(DrawSelected, () => { return selectedEditor != null; }));

      // For recompilation
      //if (selectedTrigger || selectedTriggerable)
      UpdateConnections();
    }

    protected override void OnFirstUpdate()
    {
      buttonStyle = StratusGUIStyles.button;
    }

    protected override void OnBaseEditorGUI()
    {
    }

    protected override void OnBehaviourEditorValidate()
    {
      GameObject prefab = PrefabUtility.FindPrefabRoot(target.gameObject);
      if (prefab)
      {
        StratusDebug.Log($"{target.name} has been changed");        
        PrefabUtility.RecordPrefabInstancePropertyModifications(target);
      }
    }

    private void SetTriggerContextMenu(StratusTriggerBehaviour trigger, GenericMenu menu)
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

    private void SetTriggerableContextMenu(StratusTriggerable triggerable, GenericMenu menu)
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
    private Color GetColor(StratusTriggerBase triggerBase, StratusTriggerSystem.ConnectionStatus status)
    {
      Color color = Color.white;
      switch (target.connectionDisplay)
      {
        case StratusTriggerSystem.ConnectionDisplay.Selection:
          {
            switch (status)
            {
              case StratusTriggerSystem.ConnectionStatus.Connected:
                color = connectedColor;
                break;
              case StratusTriggerSystem.ConnectionStatus.Disconnected:
                //color = Color.white;
                break;
              case StratusTriggerSystem.ConnectionStatus.Selected:
                //color = connectedColor;
                color = selectedColor;
                break;
              case StratusTriggerSystem.ConnectionStatus.Disjoint:
                color = disconnectedColor;
                break;
              default:
                break;
            }
          }
          break;

        case StratusTriggerSystem.ConnectionDisplay.Grouping:
          {
            //int colorIndex = -1;
            if (status == StratusTriggerSystem.ConnectionStatus.Selected)
              color = selectedColor;
            if (connectivityGroups.ContainsKey(triggerBase))
            {
              int colorIndex = connectivityGroups[triggerBase];
              color = StratusGUIStyles.Colors.GetDistinct(colorIndex);
            }

          }
          break;
      }

      return color;
    }

    private void SelectTrigger(StratusTriggerBehaviour trigger)
    {
      selectedTriggerable = null;
      selectedTrigger = trigger;

      // Instantiate the editor for it, disable drawwing base trigger properties
      selectedEditor = UnityEditor.Editor.CreateEditor(trigger) as StratusEditor;
      selectedEditor.backgroundStyle = EditorStyles.helpBox;

      var baseTriggerProperties = selectedEditor.GetSerializedPropertiesOfType(typeof(StratusTriggerBehaviour));
      foreach (var property in baseTriggerProperties)
      {
        // We still want to select persistence
        if (property.name == "persistent")
          continue;

        selectedEditor.propertyConstraints.Add(property, False);
      }


      //Highlighter.Highlight(nameof(TriggerSystem), trigger.GetType().Name);
    }

    private void SelectTriggerable(StratusTriggerable triggerable)
    {
      selectedTrigger = null;
      selectedTriggerable = triggerable;

      selectedEditor = UnityEditor.Editor.CreateEditor(triggerable) as StratusEditor;
      selectedEditor.backgroundStyle = EditorStyles.helpBox;
    }

    private void Deselect()
    {
      selected = null;
      selectedEditor = null;
    }

    private void RemoveTrigger(StratusTriggerBehaviour trigger)
    {
      if (selected == trigger)
      {
        Deselect();
        selectedTrigger = null;
      }
      endOfFrameRequests.Add(() =>
      {
        target.triggers.Remove(trigger);
        Undo.DestroyObjectImmediate(trigger);
      });
    }

    private void RemoveTriggerable(StratusTriggerable triggerable)
    {
      if (selected == triggerable)
      {
        Deselect();
        selectedTriggerable = null;
      }
      endOfFrameRequests.Add(() =>
      {
        target.triggerables.Remove(triggerable);
        Undo.DestroyObjectImmediate(triggerable);
      });
    }

    //------------------------------------------------------------------------/
    // Methods: Connections
    //------------------------------------------------------------------------/
    private void Connect(StratusTriggerBehaviour trigger, StratusTriggerable triggerable)
    {
      StratusDebug.Log($"Connecting {trigger.GetType().Name} and {triggerable.GetType().Name}");
      trigger.targets.Add(triggerable);
      UpdateConnections();
    }

    private void Disconnect(StratusTriggerBehaviour trigger, StratusTriggerable triggerable)
    {
      StratusDebug.Log($"Disconnecting {trigger.GetType().Name} and {triggerable.GetType().Name}");
      trigger.targets.Remove(triggerable);
      UpdateConnections();
    }

    private bool IsConnected(StratusTriggerBehaviour trigger, StratusTriggerable triggerable) => StratusTriggerSystem.IsConnected(trigger, triggerable);
    private bool IsConnected(StratusTriggerBehaviour trigger) => StratusTriggerSystem.IsConnected(trigger);
    private bool IsConnected(StratusTriggerable triggerable) => triggerableConnectivity.ContainsKey(triggerable) && triggerableConnectivity[triggerable] == StratusTriggerSystem.ConnectionStatus.Connected;

    private void UpdateConnections()
    {
      // Clear triggerables
      connectedTriggerables.Clear();
      if (selectedTrigger)
      {
        foreach (var triggerable in target.triggerables)
          connectedTriggerables.Add(triggerable, IsConnected(selectedTrigger, triggerable));
      }

      // Clear triggers
      connectedTriggers.Clear();
      if (selectedTriggerable)
      {
        foreach (var trigger in target.triggers)
          connectedTriggers.Add(trigger, IsConnected(trigger, selectedTriggerable));
      }

      // Store connectivity
      triggerableConnectivity.Clear();
      foreach (var triggerable in triggerables)
      {
        triggerableConnectivity.Add(triggerable, StratusTriggerSystem.ConnectionStatus.Disconnected);
        foreach (var trigger in triggers)
        {
          if (trigger.targets.Contains(triggerable))
          {
            triggerableConnectivity[triggerable] = StratusTriggerSystem.ConnectionStatus.Connected;
            break;
          }
        }
      }
      GenerateColorGroupings();
    }

    private void GenerateColorGroupings()
    {
      connectivityGroups.Clear();
      int groups = 1;
      foreach (var trigger in triggers)
      {
        int group = groups;

        foreach (var triggerable in trigger.targets)
        {
          if (triggerable == null)
            continue;

          if (connectivityGroups.ContainsKey(triggerable))
          {
            group = connectivityGroups[triggerable];
          }
          else
          {
            connectivityGroups.Add(triggerable, groups);
          }
        }

        connectivityGroups.Add(trigger, group);
        groups++;
      }

    }
  }

}