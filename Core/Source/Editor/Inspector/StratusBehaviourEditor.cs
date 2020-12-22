using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
	/// <summary>
	/// Base editor for all Stratus components
	/// </summary>
	public abstract class StratusBehaviourEditor<T> : StratusEditor where T : MonoBehaviour
	{
		protected new T target { get; private set; }
		protected override Type baseType => typeof(MonoBehaviour);
		protected virtual void OnBehaviourEditorValidate() { }

		internal override void OnStratusGenericEditorEnable()
		{
			this.SetTarget();
		}

		internal override void OnGenericStratusEditorValidate()
		{
			if (!this.target)
			{
				this.SetTarget();
			}

			if (this.target)
			{
				this.OnBehaviourEditorValidate();
			}
		}

		private void SetTarget()
		{
			this.target = base.target as T;
		}

	}

}