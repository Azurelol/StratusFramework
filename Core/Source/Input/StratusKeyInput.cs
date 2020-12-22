using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
    /// <summary>
    /// The state of the keyboard input
    /// </summary>
    public enum StratusKeyInputState
    {
        Down,
        Up,
        Pressed
    }

    [Serializable]
    public class StratusKeyInput : StratusInput
    {
        public KeyCode key;
        public StratusKeyInputState state;

        public override bool PollState()
        {
            bool result = false;
            switch (this.state)
            {
                case StratusKeyInputState.Down:
                    result = Input.GetKeyDown(this.key);
                    break;
                case StratusKeyInputState.Up:
                    result = Input.GetKeyUp(this.key);
                    break;
                case StratusKeyInputState.Pressed:
                    result = Input.GetKey(this.key);
                    break;
            }
            return result;
        }
    }

}