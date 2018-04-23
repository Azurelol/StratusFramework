using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using UnityEditor;

namespace Stratus
{
  [LayoutViewDisplayAttribute("Episodes", 225f, 150f, StratusGUI.Anchor.BottomRight, StratusGUI.Dimensions.Absolute)]
  public class EpisodeSceneDisplay : MultitonSceneViewDisplay<Episode>
  {
    private Dictionary<Segment, bool> segmentExpanded;

    protected override void OnInitializeMultitonState()
    {
      segmentExpanded = new Dictionary<Segment, bool>();

      foreach (var episode in Episode.availableList)
      {
        foreach (var segment in episode.get.segments)
        {
          segmentExpanded[segment] = false;
        }
      }
    }

    protected override void OnGUI(Rect position)
    {
    }

    protected override void OnGUILayout(Rect position)
    {
      ShowEpisodes();
    }

    private void ShowEpisodes()
    {
      foreach (var episode in Episode.availableList)
      {
        GUILayout.BeginVertical(GUI.skin.box);
        {
          Show(episode.get);
        }
        GUILayout.EndVertical();
      }
    }

    private void Show(Episode episode)
    {
      GUILayout.Label(episode.label, EditorStyles.whiteLargeLabel);
      //StratusEditorUtility.OnLastControlMouseClick(null, null, () => { Selection.activeGameObject = episode.gameObject; });

      EditorGUILayout.BeginHorizontal();
      StratusEditorUtility.ModifyProperty(episode, "mode", GUIContent.none);
      if (GUILayout.Button("Validate", EditorStyles.miniButtonRight))
      {
        ValidatorWindow.Open("Episode Validation", Validation.Aggregate(episode));
      }
      EditorGUILayout.EndHorizontal();

      foreach (var segment in episode.segments)
      {
        if (segment == null || !segmentExpanded.ContainsKey(segment))
          continue;

        // Show the segment
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        //episode.initialSegment == episode ? $"{episode.label} *" : episode.label, EditorStyles.whiteLabel
        GUIStyle segmentStyle = EditorStyles.label; // segment == (episode.initialSegment) ? EditorStyles.centeredGreyMiniLabel: GUI.skin.label;
        string label = episode.initialSegment == segment ? $"{segment.label} (Initial)" : segment.label;        
        segmentExpanded[segment] = EditorGUILayout.Foldout(segmentExpanded[segment], label, false, EditorStyles.foldout);

        //if (EditorGUI.Rect)
        System.Action onRightClick = () =>
        {
          var menu = new GenericMenu();
          menu.AddItem(new GUIContent("Select"), false, () => { Selection.activeGameObject = segment.gameObject; });
          if (!Application.isPlaying)
          {
            menu.AddItem(new GUIContent("Set Initial"), false, () => { episode.SetInitialSegment(segment); });
          }
          else
          {
            menu.AddItem(new GUIContent("Enter"), false, () =>
            {
              episode.Enter(segment, true, 0);
            });
          }
          menu.ShowAsContext();
        };

        System.Action onDoubleClick = () =>
        {
          Trace.Script("Boop");
          Selection.activeGameObject = segment.gameObject;
        };

        StratusEditorUtility.OnLastControlMouseClick(null, onRightClick, onDoubleClick); 

        //var button = new GUIObject();
        //button.label = segment.label;
        //button.onRightClickDown = onRightClick;
        //button.onLeftClickUp = onLeftClick;
        //button.Draw(segmentStyle);


        GUILayout.EndHorizontal();

        ShowSegment(episode, segment);
      }

    }

    private void ShowSegment(Episode episode, Segment segment)
    {
      //expanded = GUILayout.(expanded, ); //, expanded ? minimizeButtonContent : expandButtonContent); 
      //if (segment.checkpoints.NotEmpty() && GUILayout.Button(expanded ? minimizeButtonContent : expandButtonContent, EditorStyles.toggle))
      //  segmentExpanded[segment] = !expanded;
      //GUILayout.EndHorizontal();


      // Show the checkpoints
      if (!segment.checkpoints.Empty() && segmentExpanded[segment])
      {
        GUILayout.BeginVertical();
        for (int i = 0; i < segment.checkpoints.Count; ++i)
        {
          if (GUILayout.Button($"{segment.checkpoints[i].name}", EditorStyles.miniButton))
            episode.Enter(segment, true, i);
        }
        GUILayout.EndVertical();
      }
    }

  }
}