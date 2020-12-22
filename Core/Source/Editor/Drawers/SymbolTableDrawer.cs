using UnityEditor;
using UnityEngine;

namespace Stratus.Types
{
	[CustomPropertyDrawer(typeof(StratusSymbolTable), true)]
	public class SymbolTableDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty symbols = property.FindPropertyRelative("symbols");
			StratusReorderableList.List(symbols);
		}
	}
}