using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusSegment))]
  public class SegmentEditor : StratusBehaviourEditor<StratusSegment>
  {
    protected override void OnStratusEditorEnable()
    {
      //AddConstraint(nameof(Segment.onRestarted), () => target.restart);
      AddConstraint(() => target.toggleObjects, nameof(StratusSegment.toggledObjects));
    }
  }

}