namespace Stratus
{
	/// <summary>
	/// Invokes a function immediately
	/// </summary>
	public class StratusActionCall : StratusAction
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public delegate void Delegate();

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private Delegate action;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="action">A provided function which to invoke.</param>
		public StratusActionCall(Delegate action)
		{
			this.action = action;
		}

		/// <summary>
		/// Updates the action
		/// </summary>
		/// <param name="dt">The delta time.</param>
		/// <returns>How much time was consumed during this action.</returns>
		public override float Update(float dt)
		{
			if (logging)
			{
				StratusDebug.Log("#" + this.id + ": Calling function '" + this.action.Method.Name + "'");
			}

			this.action.Invoke();
			this.isFinished = true;

			if (logging)
			{
				StratusDebug.Log("#" + this.id + ": Finished!");
			}

			return 0.0f;
		}
	}


	/// <summary>
	/// Invokes a function immediately
	/// </summary>
	public class StratusActionCall<T> : StratusAction
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public delegate void Delegate<T>();

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private Delegate<T> action;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="action">A provided function which to invoke.</param>
		public StratusActionCall(Delegate<T> action)
		{
			this.action = action;
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		/// <summary>
		/// Updates the action
		/// </summary>
		/// <param name="dt">The delta time.</param>
		/// <returns>How much time was consumed during this action.</returns>
		public override float Update(float dt)
		{
			if (logging)
			{
				StratusDebug.Log("#" + this.id + ": Calling function '" + this.action.Method.Name + "'");
			}

			// If the target was destroyed in the meantime...
			if (this.action.Target == null)
			{
				return 0f;
			}

			this.action.Invoke();
			this.isFinished = true;

			if (logging)
			{
				StratusDebug.Log("#" + this.id + ": Finished!");
			}

			return 0.0f;
		}
	}

}