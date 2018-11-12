using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(Segment))]
  public class SegmentEditor : StratusBehaviourEditor<Segment>
  {
    protected override void OnStratusEditorEnable()
    {
      //AddConstraint(nameof(Segment.onRestarted), () => target.restart);
      AddConstraint(() => target.toggleObjects, nameof(Segment.toggledObjects));
    }
  }

}