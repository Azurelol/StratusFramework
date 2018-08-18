/******************************************************************************/
/*!
@file   ItemObject.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Genitus
{
  public abstract class ItemObject : MonoBehaviour
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
  public abstract class ItemObject<T> : ItemObject
  {
    public class LootEvent : Stratus.Event { public ItemObject<T> Object; }
    public T Item;
  }
}