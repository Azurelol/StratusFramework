using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// This field allows for manually selecting layers in your inspector in a safe way, implicitly converting to the layer (int)
	/// </summary>
	[Serializable]
	public class StratusLayerField
	{
		[SerializeField]
		private int layer = 0;
		public string name => LayerMask.LayerToName(layer);
		public static implicit operator string(StratusLayerField layerField) { return layerField.name; }
		public static implicit operator int(StratusLayerField layerField) { return layerField.layer; }
		public bool Matches(GameObject go) => go.layer == this;
	}

}