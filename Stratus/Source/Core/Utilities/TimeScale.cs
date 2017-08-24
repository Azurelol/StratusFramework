using UnityEngine;
using Stratus;
using System.Collections;

namespace Stratus
{
  /// <summary>
  /// 
  /// </summary>
  public enum TimeScale
  {
    Delta,
    FixedDelta,
  }

  public static class TimeScaleExtensions
  {
    /// <summary>
    /// Returns the current time based on the type (from Unity's 'Time' class)
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static float GetTime(this TimeScale scale)
    {
      float time = 0f;
      switch (scale)
      {
        case TimeScale.Delta:
          time = Time.deltaTime;
          break;
        case TimeScale.FixedDelta:
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
    public static YieldInstruction Yield(this TimeScale scale)
    {
      switch (scale)
      {
        case TimeScale.Delta:
          return new WaitForFixedUpdate();
        case TimeScale.FixedDelta:
          return new WaitForFixedUpdate();
      }

      throw new System.Exception("Unsupported scale given");
    }
  }



}