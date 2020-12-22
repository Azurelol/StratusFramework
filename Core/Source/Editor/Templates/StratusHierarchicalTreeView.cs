using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;
using UnityEditor;

namespace Stratus
{
	public abstract class StratusHierarchicalTreeView<TreeElementType> : StratusTreeViewWithTreeModel<TreeElementType>
	  where TreeElementType : StratusTreeElement
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/


		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusHierarchicalTreeView(TreeViewState state, TreeModel<TreeElementType> model) : base(state, model)
		{
			this.InitializeHierarchicalTreeView();
		}

		public StratusHierarchicalTreeView(TreeViewState state, IList<TreeElementType> data) : base(state, new TreeModel<TreeElementType>(data))
		{
			this.InitializeHierarchicalTreeView();
		}

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected virtual void OnBeforeRow(Rect rect, TreeViewItem<TreeElementType> treeViewItem) { }

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		private void InitializeHierarchicalTreeView()
		{
			this.Reload();
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnMainGUI(Rect rect)
		{
			GUI.DrawTexture(rect, EditorStyles.toolbar.normal.background);
			base.OnMainGUI(rect);
		}

		protected override void DoubleClickedItem(int id)
		{
			this.SetExpanded(id, !this.IsExpanded(id));
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			this.OnBeforeRow(args.rowRect, (TreeViewItem<TreeElementType>)args.item);
			base.RowGUI(args);
		}
	}
}
