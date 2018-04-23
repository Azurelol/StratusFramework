using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(Episode))]
  public class EpisodeEditor : BehaviourEditor<Episode>
  {
    private Episode.JumpMechanism jumpMechanism => target.mechanism;

    protected override void OnStratusEditorEnable()
    {
      AddConstraint(nameof(Episode.targetTransform), () => jumpMechanism == Episode.JumpMechanism.Translate);
      AddConstraint(nameof(Episode.onJump), () => jumpMechanism == Episode.JumpMechanism.Callback);

      AddConstraint(() => target.debugNavigation, 
        nameof(Episode.windowAnchor), 
        nameof(Episode.nextSegmentInput), 
        nameof(Episode.previousSegmentInput));
    }
  }

}