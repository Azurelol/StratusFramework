using UnityEngine;
using Stratus;

namespace Stratus
{
  /// <summary>
  /// Provides an easy way to manage cooldowns
  /// </summary>
  public class Cooldown
  {
    /// <summary>
    /// Whether this cooldown timer is currently ticking
    /// </summary>
    public bool isActive { get { return !countdown.isFinished; } }

    /// <summary>
    /// Returns the remaining duration of the cooldown
    /// </summary>
    public float remaining { get { return countdown.remaining; } }

    /// <summary>
    /// The internal countdown used by this timer
    /// </summary>
    private Countdown countdown;

    /// <param name="cooldownPeriod">How long this cooldown will take</param>
    /// <param name="startOnCooldown">Whether the cooldown should be active immediately</param>
    public Cooldown(float cooldownPeriod, bool startOnCooldown = false, Timer.Callback onFinished = null)
    {
      countdown = new Countdown(cooldownPeriod);
      if (!startOnCooldown) countdown.Finish();
      if (onFinished != null) this.countdown.SetCallback(onFinished);
    }

    /// <summary>
    /// Triggers this cooldown
    /// </summary>
    public void Activate()
    {
      countdown.Reset();
    }

    /// <summary>
    /// Updates the cooldown. Once it has finisbhed
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public bool Update(float dt)
    {
      if (countdown.Update(dt))
        return true;

      return false;
    }

  }

}