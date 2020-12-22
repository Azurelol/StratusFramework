using UnityEngine;
using Stratus.Types;

namespace Stratus.AI
{
	[CreateAssetMenu(fileName = "Goal", menuName = "Stratus/AI/Goal")]
	public class StratusGoal : ScriptableObject
	{
		/// <summary>
		/// What to do after the goal has been completed
		/// </summary>
		public enum Result
		{
			/// <summary>
			/// Do nothing when this goal is completed
			/// </summary>
			None,
			/// <summary>
			/// Reset the state
			/// </summary>
			Reset
		}

		/// <summary>
		/// The name of this goal
		/// </summary>
		[Tooltip("The name of this goal")]
		public string Name;

		/// <summary>
		/// What to do after the goal has been completed
		/// </summary>
		[Tooltip("What to do after the goal has been completed")]
		public Result OnCompletion = Result.None;

		/// <summary>
		/// The desired conditions to reach this goal.
		/// </summary>
		[Tooltip("The desired conditions to reach this goal")]
		public StratusWorldState DesiredState = new StratusWorldState();

		/// <summary>
		/// Invoked when this goal has been fulfilled
		/// </summary>
		public void Complete(Planner planner)
		{
			switch (OnCompletion)
			{
				case Result.None:
					break;
				case Result.Reset:
					this.Reset(planner);
					break;

			}
		}

		/// <summary>
		/// Resets the symbols of the planner back before the goal was completed
		/// </summary>
		/// <param name="planner"></param>
		void Reset(Planner planner)
		{

		}

	}
}