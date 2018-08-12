using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(Episode))]
  public class EpisodeEditor : BehaviourEditor<Episode>
  {
    private Episode.JumpMechanism jumpMechanism => target.mechanism;

    protected override void OnStratusEditorEnable()
    {
      AddConstraint(() => jumpMechanism == Episode.JumpMechanism.Translate, nameof(Episode.targetTransform));
      AddConstraint(() => jumpMechanism == Episode.JumpMechanism.Callback, nameof(Episode.onJump));

      AddConstraint(() => target.debugNavigation, 
        nameof(Episode.windowAnchor), 
        nameof(Episode.nextSegmentInput), 
        nameof(Episode.previousSegmentInput));
    }
  }

}