using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// An action that logs to the console
	/// </summary>
	public class StratusActionLog : StratusAction
	{
		MonoBehaviour target;
		object message;

		public StratusActionLog(object message, MonoBehaviour obj = null)
		{
			this.message = message;
			target = obj;
		}

		public override float Update(float dt)
		{
			Trace.Script(message, this.target);
			this.isFinished = true;

			if (StratusActions.debug)
				Debug.Log("#" + this.id + ": Finished!");

			return 0.0f;
		}

	}

}