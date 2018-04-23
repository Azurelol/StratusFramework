/******************************************************************************/
/*!
@file   Input.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  namespace Utilities
  {
    /// <summary>
    /// Base class for callback-based input
    /// </summary>
    public abstract class InputCallback
    {
      protected string Name;
      protected abstract void CheckInput();

      /// <summary>
      /// 
      /// </summary>
      /// <param name="name">The name of the virtual axis or button</param>
      public InputCallback(string name)
      {
        Name = name;
      }

      /// <summary>
      /// Updates the timer. When there's no cooldown remaining, it will check for 
      /// the axis input. If it is not zero, it will invoke the provided callback function.
      /// </summary>
      /// <param name="dt">The delta time.</param>
      public virtual void Update(float dt)
      {
        CheckInput();
      }
    }

    /// <summary>
    /// Input that is restricted by a timer.
    /// </summary>
    public abstract class TimedInputCallback : InputCallback
    {
      protected Stopwatch Timer;
      public float CooldownLeft { get { return Timer.current; } }
      public TimedInputCallback(string name, float cooldown) : base(name)
      {
        Timer = new Stopwatch(cooldown);
      }

      public override void Update(float dt)
      {
        Timer.Update(Time.deltaTime);
        if (Timer.isFinished)
        {
          this.CheckInput();
        }
      }

    }

    /// <summary>
    /// Cooldown-based axis input that invokes a callback when input is detected
    /// </summary>
    public class AxisInput : TimedInputCallback
    {
      public delegate void AxisCallback(float axisInput);

      private AxisCallback OnInput;

      public AxisInput(string name, float cooldown, AxisCallback onInput) : base(name, cooldown)
      {
        OnInput = onInput;
      }

      protected override void CheckInput()
      {
        var axisInput = Input.GetAxisRaw(Name);
        if (axisInput != 0f)
        {
          OnInput(axisInput);
          Timer.Reset();
        }
      }
    }

    /// <summary>
    /// Cooldown-based axis input that invokes a callback when input is detected
    /// </summary>
    public class ButtonInput : TimedInputCallback
    {
      public delegate void ButtonCallback();

      private ButtonCallback OnButtonDown;
      private ButtonCallback OnButttonUp;

      public ButtonInput(string name, float cooldown, ButtonCallback onButtonDown, ButtonCallback onButtonUp)
        : base(name, cooldown)
      {
        OnButtonDown = onButtonDown;
        OnButttonUp = onButtonUp;
      }

      public override void Update(float dt)
      {
        base.Update(dt);
        // Always check for when the button is released
        if (Input.GetButtonUp(Name) && OnButttonUp != null)
          OnButttonUp();
      }

      protected override void CheckInput()
      {
        if (Input.GetButtonDown(Name) && OnButtonDown != null)
        {
          OnButtonDown();
          Timer.Reset();
        }
      }
    }

    //public class MouseButtonInput : TimedInputCallback
    //{
    //  public delegate void MouseButtonCallback();
    //  public enum Button
    //  {
    //    Left,
    //    Middle,
    //    Right
    //  }
    //
    //  private MouseButtonCallback OnMouseButtonDown;
    //  private MouseButtonCallback OnMouseButttonUp;
    //
    //  public MouseButtonInput(Button button, float cooldown, MouseButtonCallback onMouseDown, MouseButtonCallback onMouseUp)
    //    : base(name, cooldown)
    //  {
    //    OnMouseButtonDown = onMouseDown;
    //    OnMouseButttonUp = onMouseDown;
    //  }
    //
    //  public override void Update(float dt)
    //  {
    //    base.Update(dt);
    //    // Always check for when the button is released
    //    if (Input.GetMouseButtonUp(Name))
    //      OnMouseButttonUp();
    //  }
    //
    //  protected override void CheckInput()
    //  {
    //    if (Input.GetButtonDown(Name))
    //    {
    //      OnMouseButtonDown();
    //      Timer.Reset();
    //    }
    //  }
    //}

  }
}
