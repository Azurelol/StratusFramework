using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public class StratusLabeledAction
	{
		public string label { get; private set; }
		public Action action { get; private set; }

		public StratusLabeledAction(string label, Action action)
		{
			this.label = label;
			this.action = action;
		}

		public static implicit operator Action(StratusLabeledAction action) => action.action;
	}

	public struct StratusLabeledContextAction<T> where T : class
	{
		public string label;
		public Action action;
		public T context;

		public StratusLabeledContextAction(string label, Action action, T context)
		{
			this.label = label;
			this.action = action;
			this.context = context;
		}
	}
}