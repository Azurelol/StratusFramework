using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.AI
{
  [CustomEditor(typeof(Sensor), true)]
  public class SensorEditor : BehaviourEditor<Sensor>
  {
    protected override void OnStratusEditorEnable()
    {
      SerializedProperty fovProperty = propertyMap["fieldOfView"];
      propertyConstraints.Add(fovProperty, ShowFieldOfView);
    }

    private bool ShowFieldOfView()
    {      
      if (target.mode != Sensor.DetectionMode.FieldOfView)
        return false;
      if (target.isCamera)
        return false;

      return true;
    }



  }

}