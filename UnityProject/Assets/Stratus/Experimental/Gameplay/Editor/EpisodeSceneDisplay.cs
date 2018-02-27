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
    private GUIContent expandButtonContent;
    private GUIContent minimizeButtonContent;

    protected override void OnInitializeMultitonState()
    {
      expandButtonContent = new GUIContent("+", "Show checkpoints");
      minimizeButtonContent = new GUIContent("-", "Hide checkpoints");
      segmentExpanded = new Dictionary<Segment, bool>();

      foreach (var episode in Episode.availableList)
      {
        foreach(var segment in episode.get.segments)
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
          GUILayout.Label(episode.label, EditorStyles.centeredGreyMiniLabel);
          Show(episode.get);
        }
        GUILayout.EndVertical();
      }
    }

    private void Show(Episode episode)
    {
      foreach (var segment in episode.segments)
      {
        if (segment == null)
          continue;       

        // Show the segment
        GUILayout.BeginHorizontal();
        {
          GUIStyle segmentStyle = segment == (episode.initialSegment) ? EditorStyles.whiteLabel: GUI.skin.label;

          System.Action onRightClick = () =>
          {
            var menu = new GenericMenu();
            if (!Application.isPlaying)
            {
              menu.AddItem(new GUIContent("Initial"), false, () =>
              {
                episode.SetInitialSegment(segment);
              });
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

          System.Action onLeftClick = () =>
          {
            EditorGUIUtility.PingObject(segment);
          };

          var button = new GUIObject();
          button.label = segment.label;
          button.onRightClickDown = onRightClick;
          button.onLeftClickUp = onLeftClick;
          button.Draw(segmentStyle);          
        
        }

        ShowSegment(episode, segment);
      }

    }

    private void ShowSegment(Episode episode, Segment segment)
    {
      bool expanded = segmentExpanded[segment];
      if (segment.checkpoints.NotEmpty() && GUILayout.Button(expanded ? minimizeButtonContent : expandButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(20f)))
        segmentExpanded[segment] = !expanded;
      GUILayout.EndHorizontal();


      // Show the checkpoints
      if (!segment.checkpoints.Empty() && segmentExpanded[segment])
      {
        GUILayout.BeginVertical();
        for (int i = 0; i < segment.checkpoints.Count; ++i)
        {
          if (GUILayout.Button($"{segment.checkpoints[i].name}", EditorStyles.miniButtonRight))
            episode.Enter(segment, true, i);
        }
        GUILayout.EndVertical();
      }
    }
    
  }
}