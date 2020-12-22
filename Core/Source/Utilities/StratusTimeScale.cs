using UnityEngine;
using Stratus;
using System.Collections;

namespace Stratus
{
	/// <summary>
	/// What time measure is being used
	/// </summary>
	public enum StratusTimeScale
	{
		Delta,
		FixedDelta,
	}

	public static class StratusTime
	{
		public static int minutesSinceStartup => Mathf.RoundToInt(Time.realtimeSinceStartup / 60f);

		/// <summary>
		/// Returns the current time based on the type (from Unity's 'Time' class)
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static float GetTime(this StratusTimeScale scale)
		{
			float time = 0f;
			switch (scale)
			{
				case StratusTimeScale.Delta:
					time = Time.deltaTime;
					break;
				case StratusTimeScale.FixedDelta:
					time = Time.fixedDeltaTime;
					break;
			}
			return time;
		}

		/// <summary>
		/// Returns a yield instruction which will be invoked on the next time the update is called.
		/// (Example: On fixed timescale, it will return 'WaitOnFixedUpdate'
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static YieldInstruction Yield(this StratusTimeScale scale)
		{
			switch (scale)
			{
				case StratusTimeScale.Delta:
					return new WaitForFixedUpdate();
				case StratusTimeScale.FixedDelta:
					return new WaitForFixedUpdate();
			}

			throw new System.Exception("Unsupported scale given");
		}
	}



}