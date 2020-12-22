using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.InteropServices;

namespace Stratus.UI
{
	public interface IStratusUpdatable
	{
		event Action onUpdate;
	}


	public class StratusPopUpTextBehaviour : StratusBehaviour
	{
		public StratusPopUpTransformText parameters;

		public void Trigger()
		{
			this.Log($"Instancing {parameters}");
			parameters.Instantiate();
		}


		private void OnValidate()
		{
			if (parameters == null)
			{
				parameters = new StratusPopUpTransformText(this.transform, this.name);
			}
		}
	}

}