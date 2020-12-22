using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
	[CustomEditor(typeof(StratusSensor), true)]
	public class StratusSensorEditor : StratusBehaviourEditor<StratusSensor>
	{
		protected override void OnStratusEditorEnable()
		{
			SerializedProperty fovProperty = propertyMap["fieldOfView"];
			propertyConstraints.Add(fovProperty, ShowFieldOfView);
		}

		private bool ShowFieldOfView()
		{
			if (target.mode != StratusSensor.DetectionMode.FieldOfView)
				return false;
			if (target.isCamera)
				return false;

			return true;
		}



	}

}