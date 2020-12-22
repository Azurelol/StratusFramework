using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Stratus.AI
{
		public class StratusBehaviorTreeEditorWindow : StratusEditorWindow<StratusBehaviorTreeEditorWindow>
		{
			//----------------------------------------------------------------------/
			// Declarations
			//----------------------------------------------------------------------/
			public enum Mode
			{
				Editor = 0,
				Debugger = 1,
			}

			public class BehaviourTreeView : StratusHierarchicalTreeView<BehaviorTree.BehaviorNode>
			{
				private StratusBehaviorTreeEditorWindow window => StratusBehaviorTreeEditorWindow.instance;
				private BehaviorTree tree => StratusBehaviorTreeEditorWindow.instance.behaviorTree;


				public BehaviourTreeView(TreeViewState state, IList<BehaviorTree.BehaviorNode> data) : base(state, data)
				{
				}

				protected override bool IsParentValid(BehaviorTree.BehaviorNode parent)
				{
					//Trace.Script($"Parent type = {parent.dataType}");
					if (parent.data == null)
					{
						return false;
					}

					if (parent.data is StratusAIComposite)
					{
						return true;
					}
					else if (parent.data is StratusAIDecorator && !parent.hasChildren)
					{
						return true;
					}

					return false;
				}

				protected override void OnItemContextMenu(GenericMenu menu, BehaviorTree.BehaviorNode treeElement)
				{
					// Tasks
					if (treeElement.data is StratusAITask)
					{
						BehaviorTree.BehaviorNode parent = treeElement.GetParent<BehaviorTree.BehaviorNode>();
						menu.AddItem("Duplicate", false, () => this.window.AddNode((StratusAIBehavior)treeElement.data.Clone(), parent));

						menu.AddPopup("Add/Decorator", StratusBehaviorTreeEditorWindow.decoratorTypes.displayedOptions, (int index) =>
						{
							this.window.AddParentNode(StratusBehaviorTreeEditorWindow.decoratorTypes.AtIndex(index), treeElement);
						});
					}

					// Composites
					else if (treeElement.data is StratusAIComposite)
					{
						menu.AddPopup("Add/Tasks", StratusBehaviorTreeEditorWindow.taskTypes.displayedOptions, (int index) =>
						{
							this.window.AddChildNode(StratusBehaviorTreeEditorWindow.taskTypes.AtIndex(index), treeElement);
						});

						menu.AddPopup("Add/Composites", StratusBehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
						{
							this.window.AddChildNode(StratusBehaviorTreeEditorWindow.compositeTypes.AtIndex(index), treeElement);
						});

						menu.AddPopup("Add/Decorator", StratusBehaviorTreeEditorWindow.decoratorTypes.displayedOptions, (int index) =>
						{
							this.window.AddParentNode(StratusBehaviorTreeEditorWindow.decoratorTypes.AtIndex(index), treeElement);
						});

						menu.AddPopup("Replace", StratusBehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
						{
							this.window.ReplaceNode(treeElement, StratusBehaviorTreeEditorWindow.compositeTypes.AtIndex(index));

						});
					}
					// Decorators
					else if (treeElement.data is StratusAIDecorator)
					{
						if (!treeElement.hasChildren)
						{
							menu.AddPopup("Add/Tasks", StratusBehaviorTreeEditorWindow.taskTypes.displayedOptions, (int index) =>
							{
								this.window.AddChildNode(StratusBehaviorTreeEditorWindow.taskTypes.AtIndex(index), treeElement);
							});

							menu.AddPopup("Add/Composites", StratusBehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
							{
								this.window.AddChildNode(StratusBehaviorTreeEditorWindow.compositeTypes.AtIndex(index), treeElement);
							});
						}
					}


					// Common
					if (treeElement.hasChildren)
					{
						menu.AddItem("Remove/Include Children", false, () => this.window.RemoveNode(treeElement));
						menu.AddItem("Remove/Exclude Children", false, () => this.window.RemoveNodeOnly(treeElement));
					}
					else
					{
						menu.AddItem("Remove", false, () => this.window.RemoveNode(treeElement));
					}
				}

				protected override void OnContextMenu(GenericMenu menu)
				{
					// Composite
					menu.AddPopup("Add/Composites", StratusBehaviorTreeEditorWindow.compositeTypes.displayedOptions, (int index) =>
					{
						this.window.AddChildNode(StratusBehaviorTreeEditorWindow.compositeTypes.AtIndex(index));
					});

					// Actions
					menu.AddPopup("Add/Tasks", StratusBehaviorTreeEditorWindow.taskTypes.displayedOptions, (int index) =>
					{
						this.window.AddChildNode(StratusBehaviorTreeEditorWindow.taskTypes.AtIndex(index));
					});

					menu.AddItem("Clear", false, () => this.window.RemoveAllNodes());
				}

				protected override void OnBeforeRow(Rect rect, TreeViewItem<BehaviorTree.BehaviorNode> treeViewItem)
				{
					if (treeViewItem.item.data is StratusAIComposite)
					{
						StratusGUI.GUIBox(rect, StratusAIComposite.color);
					}
					else if (treeViewItem.item.data is StratusAITask)
					{
						StratusGUI.GUIBox(rect, StratusAITask.color);
					}
					else if (treeViewItem.item.data is StratusAIDecorator)
					{
						StratusGUI.GUIBox(rect, StratusAIDecorator.color);
					}
				}
			}

			//----------------------------------------------------------------------/
			// Fields
			//----------------------------------------------------------------------/
			[SerializeField]
			private BehaviourTreeView treeInspector;
			[SerializeField]
			private TreeViewState treeViewState;
			[SerializeField]
			private Mode mode = Mode.Editor;
			[SerializeField]
			private StratusAgent selectedAgent;
			[SerializeField]
			public BehaviorTree behaviorTree;

			private StratusSerializedEditorObject currentNodeSerializedObject;
			private const string folder = "Stratus/Experimental/AI/";
			private Vector2 inspectorScrollPosition, blackboardScrollPosition;
			private StratusSerializedPropertyMap behaviorTreeProperties;
			private StratusEditor blackboardEditor;
			private string[] toolbarOptions = new string[]
			{
		nameof(Mode.Editor),
		nameof(Mode.Debugger),
			};
			

			//----------------------------------------------------------------------/
			// Properties
			//----------------------------------------------------------------------/
			private static Type compositeType { get; } = typeof(StratusAIComposite);
			private static Type decoratorType { get; } = typeof(StratusAIDecorator);

			/// <summary>
			/// All supported behavior types
			/// </summary>
			public static StratusTypeSelector behaviorTypes { get; } = StratusTypeSelector.FilteredSelector(typeof(StratusAIBehavior), typeof(StratusAIService), false, true);

			/// <summary>
			/// All supported decorator types
			/// </summary>
			public static StratusTypeSelector compositeTypes { get; } = new StratusTypeSelector(typeof(StratusAIComposite), false, true);

			/// <summary>
			/// All supported decorator types
			/// </summary>
			public static StratusTypeSelector taskTypes { get; } = new StratusTypeSelector(typeof(StratusAITask), false, true);

			/// <summary>
			/// All supported decorator types
			/// </summary>
			public static StratusTypeSelector serviceTypes { get; } = new StratusTypeSelector(typeof(StratusAIService), false, true);

			/// <summary>
			/// All supported decorator types
			/// </summary>
			public static StratusTypeSelector decoratorTypes { get; } = new StratusTypeSelector(typeof(StratusAIDecorator), false, true);

			/// <summary>
			/// The blackboard being used by the tree
			/// </summary>
			private StratusBlackboard blackboard => this.behaviorTree.blackboard;

			/// <summary>
			/// The scope of the blackboard being inspected
			/// </summary>
			private StratusBlackboard.Scope scope;

			/// <summary>
			/// The nodes currently being inspected
			/// </summary>
			public IList<BehaviorTree.BehaviorNode> currentNodes { get; private set; }

			/// <summary>
			/// The nodes currently being inspected
			/// </summary>
			public BehaviorTree.BehaviorNode currentNode { get; private set; }

			public bool hasSelection => this.currentNodes != null;

			/// <summary>
			/// Whether the editor for the BT has been initialized
			/// </summary>
			private bool isTreeSet => this.behaviorTree != null && this.behaviorTreeProperties != null;

			/// <summary>
			/// Whether the blackboard has been set
			/// </summary>
			private bool isBlackboardSet => this.blackboardEditor;

			//----------------------------------------------------------------------/
			// Messages
			//----------------------------------------------------------------------/
			protected override void OnWindowEnable()
			{
				if (this.treeViewState == null)
				{
					this.treeViewState = new TreeViewState();
				}

				if (this.behaviorTree)
				{
					this.treeInspector = new BehaviourTreeView(this.treeViewState, this.behaviorTree.tree.elements);
					this.OnTreeSet();
				}
				else
				{
					//TreeBuilder<BehaviorTree.BehaviorNode, Behavior> treeBuilder = new TreeBuilder<BehaviorTree.BehaviorNode, Behavior>();
					var tree = new StratusSerializedTree<BehaviorTree.BehaviorNode, StratusAIBehavior>();

					this.treeInspector = new BehaviourTreeView(this.treeViewState, tree.elements);
				}

				this.treeInspector.onSelectionIdsChanged += this.OnSelectionChanged;
				this.treeInspector.Reload();
			}

			protected override void OnWindowGUI()
			{
				StratusEditorGUI.BeginAligned(TextAlignment.Center);
				this.mode = (Mode)GUILayout.Toolbar((int)this.mode, this.toolbarOptions, GUILayout.ExpandWidth(false));
				StratusEditorGUI.EndAligned();

				GUILayout.Space(padding);
				switch (this.mode)
				{
					case Mode.Editor:
						this.DrawEditor();
						break;
					case Mode.Debugger:
						this.DrawDebugger();
						break;
				}

			}

			private void OnSelectionChange()
			{
				Debug.Log("Selection changed!");
				if (Selection.activeGameObject != null)
				{
					var agent = Selection.activeGameObject.GetComponent<StratusAgent>();
					if (agent != null)
					{
						OnAgentSelected(agent);
					}
				}
			}

			//----------------------------------------------------------------------/
			// Procedures
			//----------------------------------------------------------------------/
			private void DrawEditor()
			{
				//EditProperty(nameof(this.behaviorTree));
				if (this.InspectObjectFieldWithHeader(ref this.behaviorTree, "Behavior Tree"))
				{
					this.OnTreeSet();
				}

				if (!this.isTreeSet)
				{
					return;
				}

				//if (StratusEditorUtility.currentEvent.type != EventType.Repaint)
				//  return;


				Rect rect = this.currentPosition; //  this.currentPosition;
				rect = StratusEditorUtility.Pad(rect);
				rect = StratusEditorUtility.PadVertical(rect, lineHeight * 5f);

				// Hierarchy: LEFT
				rect.width *= 0.5f;
				this.DrawHierarchy(rect);

				// Inspector: TOP-RIGHT
				rect.x += rect.width;
				rect.width -= StratusEditorGUI.standardPadding;
				rect.height *= 0.5f;
				rect.height -= padding * 2f;
				this.DrawInspector(rect);

				// Blackboard: BOTTOM-RIGHT
				rect.y += rect.height;
				rect.y += padding;
				this.DrawBlackboard(rect);
			}

			private void DrawDebugger()
			{
				//EditProperty(nameof(this.agent));
				//EditProperty(nameof(debugTarget));
				this.InspectObjectFieldWithHeader(ref this.selectedAgent, "Agent");
				if (selectedAgent != null)
				{

				}
			}

			private void DrawHierarchy(Rect rect)
			{
				//if (behaviorTree != null)
				//GUI.BeginGroup(rect);
				GUILayout.BeginArea(rect);
				GUILayout.Label("Hierarchy", StratusGUIStyles.header);
				//rect.y += lineHeight * 2f;
				GUILayout.EndArea();
				//GUI.EndGroup();

				float offset = lineHeight * 3f;
				rect = StratusEditorUtility.RaiseVertical(rect, offset);
				this.treeInspector?.TreeViewGUI(rect);
				//rect = StratusEditorUtility.PadVertical(rect, lineHeight);
			}

			private void DrawInspector(Rect rect)
			{
				GUILayout.BeginArea(rect);
				GUILayout.Label("Inspector", StratusGUIStyles.header);

				if (this.hasSelection)
				{
					if (this.currentNodes.Count == 1)
					{
						GUILayout.Label(this.currentNode.dataTypeName, EditorStyles.largeLabel);
						this.inspectorScrollPosition = GUILayout.BeginScrollView(this.inspectorScrollPosition, GUI.skin.box);
						//this.currentNodeSerializedObject.DrawEditorGUI(rect);
						this.currentNodeSerializedObject.DrawEditorGUILayout();
						GUILayout.EndScrollView();
					}
					else
					{
						GUILayout.Label("Editing multiple nodes is not supported!", EditorStyles.largeLabel);
					}
				}

				GUILayout.EndArea();
			}

			private void DrawBlackboard(Rect rect)
			{
				GUILayout.BeginArea(rect);
				GUILayout.Label("Blackboard", StratusGUIStyles.header);
				{
					// Set the blackboard
					SerializedProperty blackboardProperty = this.behaviorTreeProperties.GetProperty(nameof(BehaviorTree.blackboard));
					bool changed = this.InspectProperty(blackboardProperty, "Asset");
					if (changed && this.blackboard != null)
					{
						this.OnBlackboardSet();
					}

					EditorGUILayout.Space();

					// Draw the blackboard
					if (this.blackboardEditor != null)
					{
						// Controls
						StratusEditorGUI.BeginAligned(TextAlignment.Center);
						StratusEditorGUI.EnumToolbar(ref this.scope);
						StratusEditorGUI.EndAligned();

						this.blackboardScrollPosition = EditorGUILayout.BeginScrollView(this.blackboardScrollPosition, GUI.skin.box);
						switch (this.scope)
						{
							case StratusBlackboard.Scope.Local:
								this.blackboardEditor.DrawSerializedProperty(nameof(StratusBlackboard.locals));
								break;
							case StratusBlackboard.Scope.Global:
								this.blackboardEditor.DrawSerializedProperty(nameof(StratusBlackboard.globals));
								break;
						}
						EditorGUILayout.EndScrollView();
					}

				}
				GUILayout.EndArea();
			}

			//----------------------------------------------------------------------/
			// Methods: Private
			//----------------------------------------------------------------------/
			private void AddChildNode(Type type, BehaviorTree.BehaviorNode parent = null)
			{
				if (parent != null)
				{
					this.behaviorTree.AddBehavior(type, parent);
				}
				else
				{
					this.behaviorTree.AddBehavior(type);
				}

				this.Save();
			}

			private void AddParentNode(Type type, BehaviorTree.BehaviorNode child)
			{
				this.behaviorTree.AddParentBehavior(type, child);
				this.Save();
			}

			private void AddNode(StratusAIBehavior behavior, BehaviorTree.BehaviorNode parent = null)
			{
				if (parent != null)
				{
					this.behaviorTree.AddBehavior(behavior, parent);
				}
				else
				{
					this.behaviorTree.AddBehavior(behavior);
				}

				this.Save();
			}
			private void RemoveNode(BehaviorTree.BehaviorNode node)
			{
				this.currentNodeSerializedObject = null;
				this.currentNodes = null;

				this.behaviorTree.RemoveBehavior(node);
				this.Save();
			}

			private void RemoveNodeOnly(BehaviorTree.BehaviorNode node)
			{
				this.currentNodeSerializedObject = null;
				this.currentNodes = null;

				this.behaviorTree.RemoveBehaviorExcludeChildren(node);
				this.Save();
			}

			private void ReplaceNode(BehaviorTree.BehaviorNode node, Type behaviorType)
			{
				this.currentNodeSerializedObject = null;
				this.currentNodes = null;
				this.behaviorTree.ReplaceBehavior(node, behaviorType);
				this.Save();
			}

			private void RemoveAllNodes()
			{
				this.behaviorTree.ClearBehaviors();
				this.currentNodeSerializedObject = null;
				this.currentNodes = null;
				this.Save();
			}

			private void Refresh()
			{
				this.treeInspector.SetTree(this.behaviorTree.tree.elements);
				this.Repaint();
			}

			private void OnTreeSet()
			{
				this.behaviorTree.Assert();
				this.behaviorTreeProperties = new StratusSerializedPropertyMap(this.behaviorTree, typeof(StratusScriptable));

				// Blackboard
				this.blackboardEditor = null;
				if (this.blackboard)
				{
					this.OnBlackboardSet();
				}

				this.Refresh();
			}

			private void OnBlackboardSet()
			{
				this.blackboardEditor = StratusEditor.CreateEditor(this.behaviorTree.blackboard) as StratusEditor;
			}

			private void Save()
			{
				EditorUtility.SetDirty(this.behaviorTree);
				Undo.RecordObject(this.behaviorTree, "Behavior Tree Edit");
				this.Refresh();
			}

			private void OnSelectionChanged(IList<int> ids)
			{
				this.currentNodeSerializedObject = null;
				//this.currentNodeProperty = null;

				this.currentNodes = this.treeInspector.GetElements(ids);
				if (this.currentNodes.Count > 0)
				{
					this.currentNode = this.currentNodes[0];
					this.currentNodeSerializedObject = new StratusSerializedEditorObject(this.currentNode.data);
					//SerializedObject boo = new SerializedObject(currentNode.data);
					//this.currentNodeProperty = this.treeElementsProperty.GetArrayElementAtIndex(ids[0]);
				}
			}

			private void OnAgentSelected(StratusAgent agent)
			{
				this.selectedAgent = agent;
			}

			//----------------------------------------------------------------------/
			// Methods: Static
			//----------------------------------------------------------------------/
			[OnOpenAsset]
			public static bool OnOpenAsset(int instanceID, int line)
			{
				BehaviorTree myTreeAsset = EditorUtility.InstanceIDToObject(instanceID) as BehaviorTree;
				if (myTreeAsset != null)
				{
					Open(myTreeAsset);
					return true;
				}
				return false;
			}

			public static void Open(BehaviorTree tree)
			{
				OnOpen("Behavior Tree");
				instance.SetTree(tree);
			}

			public void SetTree(BehaviorTree tree)
			{
				this.behaviorTree = tree;
				this.OnTreeSet();
			}




		}
}