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
		private Delegate DelegateInstance;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="deleg">A provided function which to invoke.</param>
		public StratusActionCall(Delegate deleg)
		{
			this.DelegateInstance = deleg;
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
				StratusDebug.Log("#" + this.id + ": Calling function '" + this.DelegateInstance.Method.Name + "'");
			}

			this.DelegateInstance.DynamicInvoke();
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
		public delegate void Delegate(T arg);

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private Delegate delegateInstance;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="deleg">A provided function which to invoke.</param>
		public StratusActionCall(Delegate deleg)
		{
			this.delegateInstance = deleg;
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
				StratusDebug.Log("#" + this.id + ": Calling function '" + this.delegateInstance.Method.Name + "'");
			}

			// If the target was destroyed in the meantime...
			if (this.delegateInstance.Target == null)
			{
				return 0f;
			}

			this.delegateInstance.DynamicInvoke();
			this.isFinished = true;

			if (logging)
			{
				StratusDebug.Log("#" + this.id + ": Finished!");
			}

			return 0.0f;
		}
	}

}