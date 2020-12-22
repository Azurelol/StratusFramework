using UnityEngine;


namespace Stratus
{
	/// <summary>
	/// A container of all actions a particular GameObject has.
	/// They propagate updates to all actions attached to it.
	/// </summary>
	public class StratusActionDriver : StratusActionGroup
	{
		//--------------------------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------------------------/
		public GameObject source { get; private set; }

		//--------------------------------------------------------------------------------------------/
		// Messags
		//--------------------------------------------------------------------------------------------/
		public StratusActionDriver(GameObject source, StratusTimeScale mode = StratusTimeScale.Delta) : base(mode)
		{
			this.source = source;
		}

		/// <summary>
		/// Updates a GameObject's actions, updating all the actions one tier below
		/// in parallel.
		/// </summary>
		/// <param name="dt">The time to be updated.</param>
		/// <returns>How much time was consumed while updating.</returns>
		public override float Update(float dt)
		{
			this.Migrate();
			return base.Update(dt);
		}
	}
}