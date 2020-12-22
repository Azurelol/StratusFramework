using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Stratus
{
	[Serializable]
	public class StratusInputActionDecorator
	{
		public string action;
		public Sprite sprite;
	}

	[CreateAssetMenu(fileName = "Stratus Input Asset Map", menuName = "Stratus/Input/Stratus Input Asset Map")]
	public class StratusInputActionDecoratorMap : StratusScriptable
	{
		[Serializable]
		public class DecoratorCollection
		{
			public StratusInputScheme scheme;
			public List<StratusInputActionDecorator> actions = new List<StratusInputActionDecorator>();

			private Dictionary<string, StratusInputActionDecorator> elementsByLabel { get; set; }

			public StratusInputActionDecorator GetAsset(string label)
			{
				if (elementsByLabel  == null)
				{
					elementsByLabel = new Dictionary<string, StratusInputActionDecorator>();
					elementsByLabel.AddRange(x => x.action, actions);
				}

				if (!elementsByLabel.ContainsKey(label))
				{
					return null;
				}

				return elementsByLabel[label];
			}
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public InputActionAsset inputActions;
		public List<DecoratorCollection> schemes = new List<DecoratorCollection>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static bool hasGamepad => Input.GetJoystickNames().Length > 0;
		private Dictionary<StratusInputScheme, DecoratorCollection> schemeMap { get; set; }

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[StratusInvokeMethod]
		public void Generate()
		{
			schemes = new List<DecoratorCollection>();
		}

		public StratusInputActionDecorator GetDecorator(StratusInputScheme inputScheme, string label)
		{
			if (schemeMap == null)
			{
				schemeMap = new Dictionary<StratusInputScheme, DecoratorCollection>();
				schemeMap.AddRange(x => x.scheme, schemes);
			}

			if (!schemeMap.ContainsKey(inputScheme))
			{
				return null;
			}

			return schemeMap[inputScheme].GetAsset(label);
		}
	}

}