using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusSegmentBehaviour))]
  public class SegmentEditor : StratusBehaviourEditor<StratusSegmentBehaviour>
  {
    protected override void OnStratusEditorEnable()
    {
      //AddConstraint(nameof(Segment.onRestarted), () => target.restart);
      AddConstraint(() => target.toggleObjects, nameof(StratusSegmentBehaviour.toggledObjects));
    }
  }

}