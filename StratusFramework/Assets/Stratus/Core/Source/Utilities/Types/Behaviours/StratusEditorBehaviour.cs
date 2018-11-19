using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// An editor-only behaviour, used for utility
	/// </summary>
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public abstract class StratusEditorBehaviour<T> : StratusBehaviour where T : MonoBehaviour
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// All currently active instances, indexed by their labels
		/// </summary>
		public static Dictionary<string, T> available { get; private set; } = new Dictionary<string, T>();
		/// <summary>
		/// Returns the first instance listed
		/// </summary>
		public static T first => availableList.FirstOrNull() as T;
		/// <summary>
		/// All currently active instances
		/// </summary>
		public static List<T> availableList { get; private set; } = new List<T>();
		/// <summary>
		/// Whether there are available segments
		/// </summary>
		public static bool hasAvailable => availableList.Count > 0;
		/// <summary>
		/// Returns the underlying class for this multiton
		/// </summary>
		public T get { get; private set; }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The identifier for this instance
		/// </summary>
		//[Tooltip("The identifier for this instance")]
		public string label => this.gameObject.name;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnStratusEditorBehaviourEnable();
		protected abstract void OnStratusEditorBehaviourDisable();
		protected abstract void OnReset();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
#if UNITY_EDITOR
		private void OnEnable()
		{
			this.get = this as T;
			available.Add(this.label, this as T);
			availableList.Add(this as T);
			this.OnStratusEditorBehaviourEnable();
		}

		private void OnDisable()
		{
			available.Remove(this.label);
			availableList.Remove(this as T);
			this.OnStratusEditorBehaviourDisable();
		}

		private void Reset()
		{
			//label = gameObject.name;
			this.OnReset();
		}
#endif

		//------------------------------------------------------------------------/
		// Editor Methods
		//------------------------------------------------------------------------/
		public static void RemoveAll()
		{
			T[] toRemove = availableList.ToArray();
			foreach (T behaviour in toRemove)
			{
				DestroyImmediate(behaviour);
			}
		}


	}

}