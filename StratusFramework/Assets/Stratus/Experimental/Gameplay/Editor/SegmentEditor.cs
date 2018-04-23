using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(Segment))]
  public class SegmentEditor : BehaviourEditor<Segment>
  {
    protected override void OnStratusEditorEnable()
    {
      //AddConstraint(nameof(Segment.onRestarted), () => target.restart);
      AddConstraint(nameof(Segment.toggledObjects), () => target.toggleObjects);
    }
  }

}