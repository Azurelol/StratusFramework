using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
    /// <summary>
    /// Provides class-specific inputs
    /// </summary>
    public interface IStratusInputProvider
    {
        StratusInputAction[] inputs { get; }
    }

    public abstract class StratusInputContext
    {
        public abstract string label { get; }
    }

    public class StratusPauseInputContext : StratusInputContext
    {
        public override string label => "Paused";
    }

    public enum StratusInputContextType
    {
        MainMenu,
        Gameplay,
        Paused
    }

    /// <summary>
    /// Provides inputs for a specific context
    /// </summary>
    /// <typeparam name="Context"></typeparam>
    public interface IStratusInputProvider<Context>: IStratusInputProvider
        where Context : StratusInputContext
    {
    }

}