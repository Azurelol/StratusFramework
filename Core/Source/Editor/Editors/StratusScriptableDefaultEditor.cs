using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(StratusScriptable), true)] 
	public class StratusScriptableDefaultEditor : StratusScriptableEditor<StratusScriptable>
	{
		protected override void OnStratusEditorEnable()
		{
		}
	}

}