using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.UI
{
	[CreateAssetMenu(menuName = "Stratus/UI/Runtime Inspector Settings")]
	public class StratusRuntimeInspectorDefaultSettings : StratusRuntimeInspectorSettings
	{
		public StratusRuntimeInspectorBooleanDrawer booleanDrawer;
		public StratusRuntimeInspectorNumericDrawer numericDrawer;
		public StratusRuntimeInspectorDropdownDrawer dropdownDrawer;
		public StratusRuntimeInspectorValueNavigationDrawer valueNavigationDrawer;

		public override StratusRuntimeInspectorDrawer GetDrawer(StratusSerializedField field)
		{
			StratusRuntimeInspectorDrawer drawer = null;
			switch (field.fieldType)
			{
				case StratusSerializedFieldType.Boolean:
					drawer = valueNavigationDrawer;
					break;
				case StratusSerializedFieldType.Integer:
					drawer = numericDrawer;
					break;
				case StratusSerializedFieldType.Float:
					drawer = numericDrawer;
					break;
				case StratusSerializedFieldType.String:
					break;
				case StratusSerializedFieldType.Enum:
					drawer = dropdownDrawer;
					break;
				case StratusSerializedFieldType.Vector2:
					break;
				case StratusSerializedFieldType.Vector3:
					break;
				case StratusSerializedFieldType.Vector4:
					break;
				case StratusSerializedFieldType.LayerMask:
					break;
				case StratusSerializedFieldType.Color:
					break;
				case StratusSerializedFieldType.Rect:
					break;
				case StratusSerializedFieldType.ObjectReference:
					break;
				case StratusSerializedFieldType.Generic:
					break;
				case StratusSerializedFieldType.Unsupported:
					break;
				default:
					break;
			}

			return drawer;
		}
	}

}