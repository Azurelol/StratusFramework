/******************************************************************************/
/*!
@file   Inventory.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class InventoryItem
  */
  /**************************************************************************/
  //[Serializable]
  //public abstract class InventoryItem<T> where T : Item
  //{
  //  //------------------------------------------------------------------------/
  //  public class LootEvent : Stratus.Event { public T Item; }
  //  public class LootedEvent : Stratus.Event { public T Item; }
  //  //------------------------------------------------------------------------/
  //  public T Item;
  //  public string Name { get { return Item.Name; } }
  //  public int Count = 1;
  //}
  //
  //public class ConsumableItem : InventoryItem<Consumable> {}
  

  /**************************************************************************/
  /*!
  @class InventoryItemCategory
  */
  /**************************************************************************/
  //public class InventoryItemCategory<T> where T : InventoryItem<Item>
  //{
  //  public Transform Object;
  //  public List<T> Items = new List<T>();
  //  public Item.Category Category;
  //
  //  public InventoryItemCategory(Inventory inventory, Item.Category category)
  //  {
  //    Category = category;
  //    this.Object = new GameObject().transform;
  //    this.Object.name = category.ToString();
  //    this.Object.transform.parent = inventory.transform;
  //
  //  }
  //
  //  public void Add(T item)
  //  {
  //    Trace.Script("Added the " + Category.ToString() + " '" + item.Name + "'");
  //    Items.Add(item);
  //    var itemObj = ItemObject.Construct(item);
  //    itemObj.transform.parent = this.Object;
  //  }
  //
  //}

  //public class InventoryItemCategories
  //{
  //  public Inventory Inventory;
  //  public Dictionary<Item.Category, InventoryItemCategory> Categories = 
  //    new Dictionary<Item.Category, InventoryItemCategory>();

  //  public InventoryItemCategories(Inventory inventory)
  //  {
  //    this.Inventory = inventory;
  //  }

  //  public void Add(Item.Category type)
  //  {
  //    Categories.Add(type, new InventoryItemCategory(type, this.Inventory));
  //  }
  //}
  
  /// <summary>
  /// A container for all items the player party is in possession of.
  /// </summary>
  public partial class Inventory : MonoBehaviour
  {    
    public class RequestEvent : Stratus.Event { public Inventory Inventory; }
    // Let's do list since serializing dictionaries in Unity is a pain @_@
    //public List<ConsumableItem> Consumables = new List<ConsumableItem>();
    public List<ConsumableEntry> Consumables = new List<ConsumableEntry>();    
    public static Inventory Current;

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    void Start()
    {
      Inventory.Current = this;
      // Subscribe to inventory events 
      Scene.Connect<ItemEntry.LootEvent>(this.OnLootEvent);
      Scene.Connect<RequestEvent>(this.OnRequestEvent);
    }

    void OnDestroy()
    {
      Inventory.Current = null;
    }

    void OnRequestEvent(RequestEvent e)
    {
      e.Inventory = this;
    }

    public void Remove(ConsumableEntry entry)
    {
      Consumables.Remove(entry);
    }


    //void OnLootEvent(ItemEntry.LootEvent e)
    //{
    //  //// If there's already an existing one, add to its count
    //  //var consumableEntry = e.Item as ConsumableEntry;
    //  //if (Consumables.Contains(consumableEntry))
    //  //{        
    //  //  Consumables.Find(x => x == consumableEntry).Count += consumableEntry.Count;
    //  //}
    //  //// Add it
    //  //else
    //  //{
    //  //  Consumables.Add(consumableEntry);
    //  //}      
    //}

    void Add<EntryType>(Item.Category category, EntryType entry)
    {
      if (category == Item.Category.Consumable)
      {

      }
    }

    void OnLootEvent(ItemEntry.LootEvent e)
    {
      //Add(e.Item);
    }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="entry"></param>
    public void Add(ConsumableEntry entry)
    {
      Consumables.Add(entry as ConsumableEntry);      
    }

    /// <summary>
    /// Adds a consumable to the inventory, constructing a new
    /// inventory item object, which holds the count.
    /// </summary>
    /// <param name="item"></param>
    public void Add(Consumable item, int count = 1)
    {
      var entry = new ConsumableEntry(item, count);
      // Now add it
      this.Add(entry);
    }

    public bool Use(ConsumableEntry consumable)
    {
      //consumable
      //Consumables.Find(x=>x == consumable)

      return false;
    }
       

    //public void Remove(Item item)
    //{
    //  Items.Remove(item.Name);
    //}

    //public void Remove(string name)
    //{
    //  Items.Remove(name);
    //}

  }

}