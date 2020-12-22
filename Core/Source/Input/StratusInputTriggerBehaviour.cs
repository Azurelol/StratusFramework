using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
	public class StratusInputTriggerBehaviour : StratusTriggerBehaviour
	{
		public StratusInputBinding input = new StratusInputBinding();
		public StratusInputBinding.Action action = StratusInputBinding.Action.Down;

		public override string automaticDescription => $"On {input} {action}";

		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{

		}

		void Update()
		{
			bool triggered = false;
			switch (action)
			{
				case StratusInputBinding.Action.Down:
					triggered = input.isDown;
					break;
				case StratusInputBinding.Action.Up:
					triggered = input.isUp;
					break;
				case StratusInputBinding.Action.Held:
					triggered = input.isPressed;
					break;
			}

			if (triggered)
				Activate();
		}
	}
}
