namespace Stratus
{
	/// <summary>
	/// A behaviour whose main messages are handled by an external manager for performance reasons.
	/// </summary>
	public abstract class ManagedBehaviour : StratusBehaviour
	{
		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected virtual void OnManagedAwake() { }
		protected virtual void OnManagedDestroy() { }
		protected virtual void OnManagedStart() { }
		protected virtual void OnManagedUpdate() { }
		protected virtual void OnManagedFixedUpdate() { }
		protected virtual void OnManagedLateUpdate() { }

		//--------------------------------------------------------------------------------------------/
		// Messages : Management
		//--------------------------------------------------------------------------------------------/
		protected internal void ManagedStart() => OnManagedStart();
		protected internal void ManagedUpdate() => OnManagedUpdate();
		protected internal void ManagedFixedUpdate() => OnManagedFixedUpdate();
		protected internal void ManagedLateUpdate() => OnManagedLateUpdate();

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		private void Awake()
		{
			ManagedBehaviourSystem.Add(this);
			this.OnManagedAwake();
		}

		private void OnDestroy()
		{
			ManagedBehaviourSystem.Remove(this);
			this.OnManagedDestroy();
		}

		//--------------------------------------------------------------------------------------------/
		// Static Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Instantiates this behaviour at runtime, adding it to the managed behaviour system
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Instantiate<T>() where T : ManagedBehaviour
		{
			T behaviour = Instantiate<T>();
			ManagedBehaviourSystem.Add(behaviour);
			return behaviour;
		}

		/// <summary>
		/// Destroys this behaviour, removing it from the managed behaviour system
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="behaviour"></param>
		public static void Destroy<T>(T behaviour) where T : ManagedBehaviour
		{
			ManagedBehaviourSystem.Remove(behaviour);
			Destroy(behaviour);
		}

	}

}