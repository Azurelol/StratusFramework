using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(SceneLinker))]
  public class SceneLinkerEditor : BehaviourEditor<SceneLinker>
  {
    protected override void OnBaseEditorEnable()
    {
      //propertyConstraints.Add(propertyMap["selectedScenes"], False);
      //drawGroupRequests.Add(new DrawGroupRequest(SelectScenes, () => { return target.scenePool != null; }));
    }
    
    
  }

}