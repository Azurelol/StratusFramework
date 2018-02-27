using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(Episode))]
  public class EpisodeEditor : BaseEditor<Episode>
  {
    private Episode.JumpMechanism jumpMechanism => target.mechanism;

    protected override void OnBaseEditorEnable()
    {
      SerializedProperty targetTransformProperty = propertyMap["targetTransform"];
      propertyConstraints.Add(targetTransformProperty, () => { return jumpMechanism == Episode.JumpMechanism.Translate; });

      SerializedProperty onJumpProperty = propertyMap["onJump"];
      propertyConstraints.Add(onJumpProperty, () => { return jumpMechanism == Episode.JumpMechanism.Callback; });
    }
  }

}