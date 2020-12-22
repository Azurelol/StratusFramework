using UnityEngine;
using Stratus;

namespace Stratus.Gameplay
{
	public abstract class StratusItemBehaviour : StratusBehaviour
	{
		public enum LootRange { Vicinity, Space }

		/// <summary>
		/// Announce whether the object was looted or not.
		/// </summary>
		[Tooltip("Announce whether the object was looted or not")]
		public bool IsAnnouncing = true;

		/// <summary>
		/// Loots the item contained by this object, adding it to the player's inventory.
		/// This will also destroy this object.
		/// </summary>
		public abstract void Loot(LootRange range);

	}

	/// <summary>
	/// A physical representation of an item in the space.
	/// </summary>
	public abstract class StratusItemBehaviour<T> : StratusItemBehaviour
		where T : StratusItem
	{
		public class LootEvent : Stratus.StratusEvent { public StratusItemBehaviour<T> Object; }
		public T item;
	}
}