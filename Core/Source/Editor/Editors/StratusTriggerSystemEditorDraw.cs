using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus.Gameplay
{
  public partial class TriggerSystemEditor : StratusBehaviourEditor<StratusTriggerSystem>
  {
    //------------------------------------------------------------------------/
    // Methods: Drawing
    //------------------------------------------------------------------------/
    private void DrawConnections(Rect rect)
    {
      // First draw the options
      DrawOptions(rect);

      //if (showMessages)
      //  DrawMessages();

      // Now display the connections
      GUILayout.BeginVertical(EditorStyles.helpBox);
      {
        columnWidth = GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f);

        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();
        GUILayout.Label("TRIGGERS", StratusGUIStyles.header);
        GUILayout.Label("TRIGGERABLES", StratusGUIStyles.header);
        GUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();
        {
          GUILayout.BeginVertical();
          {
            //EditorGUILayout.LabelField("Triggers", EditorStyles.label);
            DrawTriggers();
          }
          GUILayout.EndVertical();
          //gridDrawer.Draw(GUILayoutUtility.GetLastRect(), Vector2.zero);


          GUILayout.BeginVertical();
          {
            //EditorGUILayout.LabelField("Triggerables", EditorStyles.la);
            DrawTriggerables();
          }
          GUILayout.EndVertical();

        }
        GUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
    }

    //private void DrawMessages()
    //{
    //  EditorGUILayout.HelpBox("Boop", MessageType.Warning);
    //}

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

      DrawEditor(this.selectedEditor, this.selectedName);
    }

    private void DrawOptions(Rect rect)
    {
      EditorGUILayout.BeginHorizontal();

      // Add Menu
      if (GUILayout.Button(StratusGUIStyles.addIcon, StratusGUIStyles.smallLayout))
      {
        var menu = new GenericMenu();
        menu.AddPopup("Add Trigger", triggerTypes.displayedOptions, (int index) =>
        {
          target.gameObject.AddComponent(triggerTypes.AtIndex(index));
          UpdateConnections();
        });
        menu.AddPopup("Add Triggerable", triggerableTypes.displayedOptions, (int index) =>
        {
          target.gameObject.AddComponent(triggerableTypes.AtIndex(index));
          UpdateConnections();
        });
        menu.ShowAsContext();
      }

      //if (GUILayout.Button(new GUIContent($"{messages.Count}", StratusGUIStyles.messageTexture), StratusGUIStyles.smallLayout))
      //{
      //}

      // Validation
      if (GUILayout.Button(StratusGUIStyles.validateIcon, StratusGUIStyles.smallLayout))
      {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Validate All"), false, ()=> Validate(ValidateAll));
        menu.AddItem(new GUIContent("Validate Trigger Persistence"), false, ()=>Validate(ValidatePersistence));
        menu.AddItem(new GUIContent("Validate Connections"), false, ()=>Validate(ValidateConnections));
        menu.AddItem(new GUIContent("Validate Null"), false, () => Validate(ValidateNull));
        menu.ShowAsContext();
      }

      // Options Menu
      if (GUILayout.Button(StratusGUIStyles.optionsIcon, StratusGUIStyles.smallLayout))
      {
        var menu = new GenericMenu();
        menu.AddEnumToggle<StratusTriggerSystem.ConnectionDisplay>(propertyMap[nameof(StratusTriggerSystem.connectionDisplay)]);
        menu.AddBooleanToggle(propertyMap[nameof(StratusTriggerSystem.showDescriptions)]);
        menu.AddBooleanToggle(propertyMap[nameof(StratusTriggerSystem.outlines)]);
        menu.ShowAsContext();
      }

      EditorGUILayout.EndHorizontal();
    }

    private void DrawTrigger(StratusTriggerBehaviour trigger)
    {
      StratusTriggerSystem.ConnectionStatus status = StratusTriggerSystem.ConnectionStatus.Disconnected;
      if (selected)
      {
        if (selected == trigger)
          status = StratusTriggerSystem.ConnectionStatus.Selected;
        else if (selectedTriggerable && connectedTriggers.ContainsKey(trigger) && connectedTriggers[trigger])
          status = StratusTriggerSystem.ConnectionStatus.Connected;
      }

      if (!IsConnected(trigger) && selected != trigger)
        status = StratusTriggerSystem.ConnectionStatus.Disjoint;

      Color color = GetColor(trigger, status);
      Draw(trigger, color, SelectTrigger, RemoveTrigger, SetTriggerContextMenu);
    }





    private void DrawTriggerable(StratusTriggerable triggerable)
    {
      StratusTriggerSystem.ConnectionStatus status = StratusTriggerSystem.ConnectionStatus.Disconnected;
      if (selected)
      {
        if (selected == triggerable)
          status = StratusTriggerSystem.ConnectionStatus.Selected;
        else if (selectedTrigger && connectedTriggerables.ContainsKey(triggerable) && connectedTriggerables[triggerable])
          status = StratusTriggerSystem.ConnectionStatus.Connected;
      }
      if (!IsConnected(triggerable) && selected != triggerable)
        status = StratusTriggerSystem.ConnectionStatus.Disjoint;

      Color color = GetColor(triggerable, status);
      Draw(triggerable, color, SelectTriggerable, RemoveTriggerable, SetTriggerableContextMenu);
    }

    private void Draw<T>(T baseTrigger, Color backgroundColor, System.Action<T> selectFunction, System.Action<T> removeFunction, System.Action<T, GenericMenu> contextMenuSetter) where T : StratusTriggerBase
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
        //// Reset
        //menu.AddItem(new GUIContent("Reset"), false, () => { baseTrigger.Invoke("Reset", 0f); });
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
      };

      System.Action<object> onDrop = (object other) =>
      {
        if (baseTrigger is StratusTriggerBehaviour)
        {
          if (other is StratusTriggerBehaviour)
            triggerSwapOperations.Add(new Tuple<StratusTriggerBehaviour, StratusTriggerBehaviour>(baseTrigger as StratusTriggerBehaviour, other as StratusTriggerBehaviour));
          else if (other is StratusTriggerable)
            Connect(baseTrigger as StratusTriggerBehaviour, other as StratusTriggerable);
        }
        else if (baseTrigger is StratusTriggerable)
        {
          if (other is StratusTriggerable)
            triggerableSwapOperations.Add(new Tuple<StratusTriggerable, StratusTriggerable>(baseTrigger as StratusTriggerable, other as StratusTriggerable));
          else if (other is StratusTriggerBehaviour)
            Connect(other as StratusTriggerBehaviour, baseTrigger as StratusTriggerable);
        }
      };

      Func<object, bool> onValidateDrag = (object other) =>
      {
        if (other is StratusTriggerBehaviour || other is StratusTriggerable)
          return true;
        return false;
      };

      var button = new StratusGUIObject();
      button.label = baseTrigger.enabled ? name : $"<color=grey>{name}</color>";
      button.description = $"<i><size={descriptionSize}>{baseTrigger.description}</size></i>";
      button.showDescription = showDescriptions;
      button.descriptionsWithLabel = target.descriptionsWithLabel;
      button.tooltip = baseTrigger.description;
      button.onLeftClickUp = onLeftClick;
      button.onRightClickDown = onRightClick;
      button.onDrag = onDrag;
      button.onDrop = onDrop;
      button.dragDataIdentifier = "Stratus Trigger System Button";
      button.dragData = baseTrigger;
      button.onValidateDrag = onValidateDrag;
      button.AddKey(KeyCode.Delete, () =>
      {
        removeFunction(baseTrigger);
        UpdateConnections();
      });
      button.isSelected = selected == baseTrigger;
      //button.outlineColor = backgroundColor;

      if (target.outlines)
      {
        button.backgroundColor = Color.white;
        button.Draw(buttonStyle, columnWidth);
        StratusGUIStyles.DrawOutline(button.rect, backgroundColor, baseTrigger is StratusTriggerBehaviour ? StratusGUIStyles.Border.Right : StratusGUIStyles.Border.Left);
      }
      else
      {
        button.backgroundColor = backgroundColor;
        button.outlineColor = Color.black;
        button.Draw(buttonStyle, columnWidth);
      }

    }

  }

}