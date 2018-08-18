using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Genitus
{
  public partial class Inventory : MonoBehaviour
  {
    [Serializable]
    public class ItemEntry
    {
      //------------------------------------------------------------------------/
      public class LootEvent : Stratus.Event { public ItemEntry Item; }
      public class LootedEvent : Stratus.Event { public ItemEntry Item; }
      //------------------------------------------------------------------------/
      public Item Item;
      public string Name { get { return Item.Name; } }
      public int Count = 1;
      public ItemEntry(Item item, int count)
      {
        Item = item;
        Count = count;
      }
    }

    /// <summary>
    /// A container for a consumable belonging in the inventory
    /// </summary>
    [Serializable]
    public class ConsumableEntry
    {
      public Consumable Item;
      public string Name { get { return Item.Name; } }
      /// <summary>
      /// Used by regular, expendable items.
      /// </summary>
      public int Count;
      /// <summary>
      /// Used by persistent items.
      /// </summary>
      public int Charges;
      public ConsumableEntry(Consumable item, int count)
      {
        Item = item;
        Count = count;
        Charges = item.charges;
      }

      /// <summary>
      /// Validates whether the item can be used or not.
      /// </summary>
      /// <returns></returns>
      public bool Validate()
      {
        if (Item.persistent)
        {
          if (Charges > 1)
            return true;
          return false;
        }

        // Expendable items can always be used
        return true;
      }

      /// <summary>
      /// Uses the item.
      /// </summary>
      /// <param name="user"></param>
      /// <param name="target"></param>
      public void Use(CombatController user, CombatController[] targets)
      {
        Item.Use(user, targets);

        // If the item is persistent, consume a charge
        if (Item.persistent)
        {
          Charges--;
        }
        // Otherwise, remove one
        else if (!Item.persistent)
        {
          Count--;
        }
        // If there's no more left, remove this entry
        if (Count < 1)
          Remove();
      }

      /// <summary>
      /// Removes this consumable from the inventory.
      /// </summary>
      void Remove()
      {
        Inventory.Current.Remove(this);
      }

      /// <summary>
      /// Resets all charges on the consumable
      /// </summary>
      public void Reset()
      {
        Charges = Item.charges;
      }
    }

  }

}