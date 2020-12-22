using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus.UI
{
	public class StratusRuntimeInspector : StratusBehaviour, IStratusSelectableParent
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private StratusRuntimeInspectorSettings settings;
		[SerializeField]
		private StratusLayoutElementStyle style = new StratusLayoutElementStyle();
		[SerializeField]
		private VerticalLayoutGroup layoutGroup;
		[SerializeField]
		private bool explicitNavigation = true;
		[SerializeField]
		private bool redirectInputToDrawer = true;
		[SerializeField]
		private float drawerWidth = 300;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public StratusSerializedObject serializedObject { get; private set; }
		public object inspectedObject => serializedObject.target;

		public StratusSelectable activeSelectable { get; private set; }

		private List<StratusRuntimeInspectorFieldBehaviour> serializedFieldObjects = new List<StratusRuntimeInspectorFieldBehaviour>();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Inspect(object obj)
		{
			Clear();
			serializedObject = new StratusSerializedObject(obj);
			GenerateDrawers();
			if (explicitNavigation)
			{
				SetExplicitNavigation();
			}
		}

		public void Clear()
		{
			serializedObject = null;
			serializedFieldObjects.DestroyGameObjectsAndClear();
		}

		public void Refresh()
		{

		}

		public void Select()
		{
			serializedFieldObjects.First().Select();

		}

		public void Deselect()
		{

		}

		public void SendInputToDrawer(Vector2 dir)
		{
			if (redirectInputToDrawer)
			{
				activeSelectable?.Navigate(dir);
			}
		}

		public void OnSelected(StratusSelectable selectable)
		{
			this.activeSelectable = selectable;
		}

		public void OnDeselected(StratusSelectable selectable)
		{
			this.activeSelectable = null;
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void GenerateDrawers()
		{
			foreach (var field in serializedObject.fields)
			{
				GenerateDrawer(field, null);
			}
		}

		private void GenerateDrawer(StratusSerializedField field, StratusRuntimeInspectorFieldBehaviour parent)
		{
			StratusRuntimeInspectorFieldBehaviour current = Instantiate(settings.fieldBehaviour, layoutGroup.transform, false);
			current.SetParent(this);
			serializedFieldObjects.Add(current);

			// Set the drawer (if available)
			// The drawer selected can change depending on attributes specified for the field
			StratusRuntimeInspectorDrawer drawer = null;
			StratusRuntimeInspectorDrawer drawerPrefab = settings.GetDrawer(field);
			if (drawerPrefab != null)
			{
				drawer = Instantiate(drawerPrefab);
			}

			StratusRuntimeInspectorFieldSettings fieldBehaviourSettings = new StratusRuntimeInspectorFieldSettings();
			fieldBehaviourSettings.field = field;
			fieldBehaviourSettings.drawer = drawer;
			fieldBehaviourSettings.parent = parent;
			fieldBehaviourSettings.drawerWidth = drawerWidth;
			fieldBehaviourSettings.style = this.style;
			current.Set(fieldBehaviourSettings);

			// Offset by depth somehow?
			int depth = parent != null ? parent.depth : 0;
			if (parent != null)
			{
			}

			// Get the drawer
			if (field.hasChildren)
			{
				foreach (var child in field.children)
				{
					GenerateDrawer(child, current);
				}
			}
		}

		private void SetExplicitNavigation()
		{
			StratusCanvasUtility.SetExplicitNavigation(serializedFieldObjects, x => x.selectable, StratusOrientation.Vertical);
		}
	}

}