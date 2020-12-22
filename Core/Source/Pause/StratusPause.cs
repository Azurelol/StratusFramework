using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
    /// <summary>
    /// Pauses the simulation
    /// </summary>
    public class StratusPauseEvent : StratusEvent
    {
    }

    /// <summary>
    /// Resumes the simulation
    /// </summary>
    public class StratusResumeEvent : StratusEvent 
    {
    }

    /// <summary>
    /// Default options for pausing
    /// </summary>
    [Serializable]
    public class StratusPauseOptions
    {
        public bool timeScale = true;
    }
}