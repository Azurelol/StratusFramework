using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System;

namespace Stratus
{
  [CustomEditor(typeof(TriggerSystem))]
  public partial class TriggerSystemEditor : BehaviourEditor<TriggerSystem>
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
    private List<Tuple<Trigger, Trigger>> triggerSwapOperations = new List<Tuple<Trigger, Trigger>>();
    private List<Tuple<Triggerable, Triggerable>> triggerableSwapOperations = new List<Tuple<Triggerable, Triggerable>>();
    private GUILayoutOption columnWidth;
    private TypeSelector triggerTypes, triggerableTypes;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    private StratusEditor selectedEditor { get; set; }
    private string selectedName { get; set; }
    private Triggerable selectedTriggerable { get; set; }
    private Trigger selectedTrigger { get; set; }
    private TriggerBase selected { get; set; }
    private Dictionary<Triggerable, bool> connectedTriggerables { get; set; } = new Dictionary<Triggerable, bool>();
    private Dictionary<Trigger, bool> connectedTriggers { get; set; } = new Dictionary<Trigger, bool>();
    public Dictionary<Triggerable, TriggerSystem.ConnectionStatus> triggerableConnectivity { get; private set; } = new Dictionary<Triggerable, TriggerSystem.ConnectionStatus>();
    public List<Trigger> triggers => target.triggers;
    public List<Triggerable> triggerables => target.triggerables;
    public Dictionary<TriggerBase, int> connectivityGroups { get; set; } = new Dictionary<TriggerBase, int>();
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
      //graph = SingletonAsset.LoadOrCreate<TriggerSystemEditorGraph>("Trigger System Graph", "Stratus/Core/Resources", false);
      //graph.Initialize(this);
      //XNodeEditor.NodeEditorWindow.OnOpen(graph.GetInstanceID(), 0);
      ////graphEditor = XNodeEditor.NodeGraphEditor.GetEditor(graph);

      triggerTypes = new TypeSelector(typeof(Trigger), true);
      triggerableTypes = new TypeSelector(typeof(Triggerable), true);

      selectedColor = StratusGUIStyles.selectedColor;
      connectedColor = StratusGUIStyles.connectedColor;
      disconnectedColor = StratusGUIStyles.disconnectedColor;

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

    public override void OnBaseEditorGUI()
    {
    }

    protected override void OnBehaviourEditorValidate()
    {
      GameObject prefab = PrefabUtility.FindPrefabRoot(target.gameObject);
      if (prefab)
      {
        Trace.Script($"{target.name} has been changed");        
        PrefabUtility.RecordPrefabInstancePropertyModifications(target);
      }
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
    private Color GetColor(TriggerBase triggerBase, TriggerSystem.ConnectionStatus status)
    {
      Color color = Color.white;
      switch (target.connectionDisplay)
      {
        case TriggerSystem.ConnectionDisplay.Selection:
          {
            switch (status)
            {
              case TriggerSystem.ConnectionStatus.Connected:
                color = connectedColor;
                break;
              case TriggerSystem.ConnectionStatus.Disconnected:
                //color = Color.white;
                break;
              case TriggerSystem.ConnectionStatus.Selected:
                //color = connectedColor;
                color = selectedColor;
                break;
              case TriggerSystem.ConnectionStatus.Disjoint:
                color = disconnectedColor;
                break;
              default:
                break;
            }
          }
          break;

        case TriggerSystem.ConnectionDisplay.Grouping:
          {
            //int colorIndex = -1;
            if (status == TriggerSystem.ConnectionStatus.Selected)
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

    private void SelectTrigger(Trigger trigger)
    {
      selectedTriggerable = null;
      selectedTrigger = trigger;

      // Instantiate the editor for it, disable drawwing base trigger properties
      selectedEditor = UnityEditor.Editor.CreateEditor(trigger) as StratusEditor;
      selectedEditor.backgroundStyle = EditorStyles.helpBox;

      var baseTriggerProperties = selectedEditor.propertiesByType[typeof(Trigger)];
      foreach (var property in baseTriggerProperties)
      {
        // We still want to select persistence
        if (property.name == "persistent")
          continue;

        selectedEditor.propertyConstraints.Add(property, False);
      }


      //Highlighter.Highlight(nameof(TriggerSystem), trigger.GetType().Name);
    }

    private void SelectTriggerable(Triggerable triggerable)
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

    private void RemoveTrigger(Trigger trigger)
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

    private void RemoveTriggerable(Triggerable triggerable)
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
    private void Connect(Trigger trigger, Triggerable triggerable)
    {
      Trace.Script($"Connecting {trigger.GetType().Name} and {triggerable.GetType().Name}");
      trigger.targets.Add(triggerable);
      UpdateConnections();
    }

    private void Disconnect(Trigger trigger, Triggerable triggerable)
    {
      Trace.Script($"Disconnecting {trigger.GetType().Name} and {triggerable.GetType().Name}");
      trigger.targets.Remove(triggerable);
      UpdateConnections();
    }

    private bool IsConnected(Trigger trigger, Triggerable triggerable) => TriggerSystem.IsConnected(trigger, triggerable);
    private bool IsConnected(Trigger trigger) => TriggerSystem.IsConnected(trigger);
    private bool IsConnected(Triggerable triggerable) => triggerableConnectivity.ContainsKey(triggerable) && triggerableConnectivity[triggerable] == TriggerSystem.ConnectionStatus.Connected;

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
        triggerableConnectivity.Add(triggerable, TriggerSystem.ConnectionStatus.Disconnected);
        foreach (var trigger in triggers)
        {
          if (trigger.targets.Contains(triggerable))
          {
            triggerableConnectivity[triggerable] = TriggerSystem.ConnectionStatus.Connected;
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

    //private void OpenWindow()
    //{
    //  canvas = SingletonAsset.LoadOrCreate<TriggerSystemCanvas>("Trigger System Canvas", "Stratus/Core/Resources", false, NodeEditorFramework.NodeCanvas.Initialize);
    //  NodeEditorFramework.Standard.NodeEditorWindow.OpenNodeEditor(canvas);
    //}


  }

}