using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.OdinSerializer;
using System.Linq;

namespace Stratus
{
	/// <summary>
	/// A serialized element of a Tree Model
	/// </summary>
	[Serializable]
	public partial class StratusTreeElement
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/ 
		[SerializeField] public int id;
		[SerializeField] public string name;
		[SerializeField] public int depth;

		[NonSerialized] public StratusTreeElement parent;
		[NonSerialized] public List<StratusTreeElement> children;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/ 
		/// <summary>
		/// Whether this tree element has children
		/// </summary>
		public bool hasChildren => children != null && children.Count > 0;
		/// <summary>
		/// The root node must have a depth of -1
		/// </summary>
		public bool isRoot => depth == -1;
		/// <summary>
		/// Howw many children this element has
		/// </summary>
		public int childrenCount => children != null ? children.Count : 0;
		/// <summary>
		/// How many children in total this element has (including subchildren)
		/// </summary>
		public int totalChildrenCount => GetTotalChildrenCount(this);
		/// <summary>
		/// How many children in total this element has (including subchildren)
		/// </summary>
		public StratusTreeElement[] allChildren => GetAllChildren(this);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/ 
		public StratusTreeElement()
		{
		}

		public StratusTreeElement(string name, int depth, int id)
		{
			this.name = name;
			this.depth = depth;
			this.id = id;
		}

		public override string ToString()
		{
			return $"{name} id({id}) depth({depth})";
		}
	}

	/// <summary>
	/// Generic class for a tree element with one primary data member
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public abstract class TreeElement<DataType> : StratusTreeElement, ISerializationCallbackReceiver
	  where DataType : class, IStratusLabeled
	{
		//----------------------------------------------------------------------/
		// Fields
		//----------------------------------------------------------------------/
		[OdinSerialize]
		public DataType data;
		[OdinSerialize]
		public string dataTypeName;

		//----------------------------------------------------------------------/
		// Properties
		//----------------------------------------------------------------------/    
		public bool hasData => data != null;
		public Type dataType => data.GetType();
		public DataType[] childrenData { get; private set; }

		//----------------------------------------------------------------------/
		// Messages
		//----------------------------------------------------------------------/    
		public void OnBeforeSerialize()
		{
			if (hasData)
				this.UpdateName();
		}

		public void OnAfterDeserialize()
		{
			this.childrenData = this.GetChildrenData();
		}

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/

		public void Set(DataType data)
		{
			this.data = data;
			this.dataTypeName = data.GetType().Name;
			this.UpdateName();
		}

		public void UpdateName()
		{
			this.name = this.GetName();
		}

		protected virtual string GetName()
		{
			return data.label;
		}

		public DataType[] GetChildrenData()
		{
			if (!this.hasChildren)
				return null;

			List<DataType> children = new List<DataType>();
			foreach (var child in this.children)
				children.Add(((TreeElement<DataType>)child).data);
			return children.ToArray();
		}

		public TreeElement<DataType> GetChild(int index) => (TreeElement<DataType>)children[index];
		public T GetParent<T>() where T : StratusTreeElement => (T)parent;





	}

}