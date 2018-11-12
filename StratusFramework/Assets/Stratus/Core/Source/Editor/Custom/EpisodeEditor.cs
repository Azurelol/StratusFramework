using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusEpisode))]
  public class EpisodeEditor : StratusBehaviourEditor<StratusEpisode>
  {
    private StratusEpisode.JumpMechanism jumpMechanism => target.mechanism;

    protected override void OnStratusEditorEnable()
    {
      AddConstraint(() => jumpMechanism == StratusEpisode.JumpMechanism.Translate, nameof(StratusEpisode.targetTransform));
      AddConstraint(() => jumpMechanism == StratusEpisode.JumpMechanism.Callback, nameof(StratusEpisode.onJump));

      AddConstraint(() => target.debugNavigation, 
        nameof(StratusEpisode.windowAnchor), 
        nameof(StratusEpisode.nextSegmentInput), 
        nameof(StratusEpisode.previousSegmentInput));
    }
  }

}