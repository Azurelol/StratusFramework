namespace Stratus
{
	/// <summary>
	/// An ActionSequence is a type of set that updates all its actions
	/// and children in sequence, depleting its time slice as it updates
	/// each.
	/// </summary>
	public class StratusActionGroup : StratusActionSet
	{
		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusActionGroup(StratusTimeScale mode = StratusTimeScale.Delta) : base(mode) { }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		/// <summary>
		/// Updates an ActionGroup, by updating the actions in the group in
		/// parallel.
		/// </summary>
		/// <param name="dt">The time to be updated.</param>
		/// <returns>How much time was consumed while updating.</returns>
		public override float Update(float dt)
		{
			this.Migrate();

			float mostTimeElapsed = 0.0f;

			// In an ActionGroup, every action is updated in parallel, given the same 
			// time slice.
			foreach (StratusAction action in this.activeActions)
			{
				// If an action is inactive, continue to the next one
				//if (!action.isActive)
				//  continue;

				// Every action consumes time from the time slice given (dt)
				float timeElapsed = action.Update(dt);
				// If this action took longer than the previous action, it is the new maximum
				if (timeElapsed > mostTimeElapsed)
				{
					mostTimeElapsed = timeElapsed;
				}
			}

			// Sweep inactive actions
			this.Sweep();

			return mostTimeElapsed;
		}
	}

}
