using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Stratus.Gameplay
{
	public abstract class StratusItem : StratusScriptable
	{
		public string description;
		public float value;
		public bool unique;
		public Sprite icon;

		public abstract string category { get; }
	}
}