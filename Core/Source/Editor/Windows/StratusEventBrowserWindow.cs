using System;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Displays all present derived Stratus events in the assembly
	/// </summary>
	public class StratusEventBrowserWindow : StratusEditorWindow<StratusEventBrowserWindow>
	{
		//------------------------------------------------------------------------/
		// Tree View
		//------------------------------------------------------------------------/
		/// <summary>
		/// Basic information about an event
		/// </summary>
		public class EventInformation : IStratusLabeled
		{
			public string @namespace;
			public string @class;
			public string name;
			public string members;
			private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			string IStratusLabeled.label => this.name;

			public EventInformation(System.Type type)
			{
				this.name = type.Name;
				this.@class = type.DeclaringType != null ? type.DeclaringType.Name : string.Empty;
				this.@namespace = type.Namespace;
				List<string> members = new List<string>();
				members.AddRange(type.GetFields(bindingFlags).ToStringArray((FieldInfo member) => $"({member.FieldType.Name}) {member.Name}"));
				members.AddRange(type.GetProperties(bindingFlags).ToStringArray((PropertyInfo member) => $"({member.PropertyType.Name}) {member.Name}"));
				this.members = string.Join(", ", members);
			}

		}

		public class EventTreeElement : TreeElement<EventInformation>
		{
		}

		public enum Columns
		{
			Namespace,
			Class,
			Name,
			Members,
		}

		public class EventTreeView : MultiColumnTreeView<EventTreeElement, Columns>
		{
			public EventTreeView(TreeViewState state, IList<EventTreeElement> data) : base(state, data)
			{
			}

			protected override TreeViewColumn BuildColumn(Columns columnType)
			{
				TreeViewColumn column = null;
				switch (columnType)
				{
					case Columns.Name:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Name"),
							sortedAscending = true,
							width = 150,
							autoResize = true,
							selectorFunction = (TreeViewItem<EventTreeElement> element) => element.item.data.name
						};
						break;
					case Columns.Class:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Class"),
							sortedAscending = true,
							width = 150,
							autoResize = true,
							selectorFunction = (TreeViewItem<EventTreeElement> element) => element.item.data.members
						};
						break;
					case Columns.Members:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Members"),
							sortedAscending = true,
							width = 450,
							autoResize = true,
							selectorFunction = (TreeViewItem<EventTreeElement> element) => element.item.data.members
						};
						break;
					case Columns.Namespace:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Namespace"),
							width = 175,
							autoResize = true,
							sortedAscending = true,
							selectorFunction = (TreeViewItem<EventTreeElement> element) => element.item.data.@namespace
						};
						break;
				}
				return column;
			}

			protected override void DrawColumn(Rect cellRect, TreeViewItem<EventTreeElement> item, Columns column, ref RowGUIArgs args)
			{
				switch (column)
				{
					case Columns.Name:
						DefaultGUI.Label(cellRect, item.item.data.name, args.selected, args.focused);
						break;
					case Columns.Class:
						DefaultGUI.Label(cellRect, item.item.data.@class, args.selected, args.focused);
						break;
					case Columns.Members:
						DefaultGUI.Label(cellRect, item.item.data.members, args.selected, args.focused);
						break;
					case Columns.Namespace:
						DefaultGUI.Label(cellRect, item.item.data.@namespace, args.selected, args.focused);
						break;
				}
			}

			protected override Columns GetColumn(int index)
			{
				return (Columns)index;
			}

			protected override int GetColumnIndex(Columns columnType)
			{
				return (int)columnType;
			}

			protected override void OnContextMenu(GenericMenu menu)
			{

			}

			protected override void OnItemContextMenu(GenericMenu menu, EventTreeElement treeElement)
			{
				menu.AddItem(new GUIContent("Open file"), false, () =>
				{

				});
			}
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private TreeViewState treeViewState = new TreeViewState();
		[SerializeField]
		private EventTreeView treeView;
		[SerializeField]
		private StratusSerializedTree<EventTreeElement, EventInformation> tree;

		private Vector2 scrollPosition;
		private Type[] events;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowEnable()
		{
			this.treeView = new EventTreeView(this.treeViewState, this.BuildEventTree());
		}

		protected override void OnWindowGUI()
		{
			this.treeView.TreeViewGUI(this.guiPosition);
		}

		[MenuItem("Stratus/Core/Event Browser")]
		private static void Open()
		{
			OnOpen("Event Browser");
		}

		//------------------------------------------------------------------------/
		// Data
		//------------------------------------------------------------------------/
		private List<EventTreeElement> BuildEventTree()
		{
			this.events = StratusReflection.GetSubclass<Stratus.StratusEvent>();
			EventInformation[] eventsInformation = new EventInformation[this.events.Length];
			for (int i = 0; i < this.events.Length; ++i)
			{
				eventsInformation[i] = new EventInformation(this.events[i]);
			}

			//var treeBuilder = new TreeBuilder<EventTreeElement, EventInformation>();
			//treeBuilder.AddChildren(eventsInformation, 0);
			//return treeBuilder.ToTree();

			tree = new StratusSerializedTree<EventTreeElement, EventInformation>(eventsInformation);
			return tree.elements;
		}

	}

}