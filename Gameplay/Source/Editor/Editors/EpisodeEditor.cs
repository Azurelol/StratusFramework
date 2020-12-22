using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusEpisodeBehaviour))]
  public class EpisodeEditor : StratusBehaviourEditor<StratusEpisodeBehaviour>
  {
    private StratusEpisodeBehaviour.JumpMechanism jumpMechanism => target.mechanism;

    protected override void OnStratusEditorEnable()
    {
      AddConstraint(() => jumpMechanism == StratusEpisodeBehaviour.JumpMechanism.Translate, nameof(StratusEpisodeBehaviour.targetTransform));
      AddConstraint(() => jumpMechanism == StratusEpisodeBehaviour.JumpMechanism.Callback, nameof(StratusEpisodeBehaviour.onJump));

      AddConstraint(() => target.debugNavigation, 
        nameof(StratusEpisodeBehaviour.windowAnchor), 
        nameof(StratusEpisodeBehaviour.nextSegmentInput), 
        nameof(StratusEpisodeBehaviour.previousSegmentInput));
    }
  }

}