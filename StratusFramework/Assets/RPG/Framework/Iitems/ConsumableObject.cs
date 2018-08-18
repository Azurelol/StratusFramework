/******************************************************************************/
/*!
@file   ConsumableObject.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Altostratus
{
  /// <summary>
  /// 
  /// </summary>
  public class ConsumableObject : ItemObject<Inventory.ConsumableEntry>
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public override void Loot(LootRange range)
    {
      // Send the loot event to the space. Any inventories will pick it up.
      var lootEvent = new LootEvent();
      lootEvent.Object = this;
      if (range == LootRange.Space)
      {
        Scene.Dispatch<LootEvent>(lootEvent);
      }

      // Send a system message?
      if (IsAnnouncing)
      {
        //var message = new Message();
        //message.Title = "Item looted!";
        //message.Description = "You looted <b>" + Item.Name + "</b> !";
        //SystemMessageDispatcher.Dispatch(message);
      }

      // Depending on the type of item looted, send a loot event
      Destroy(this.gameObject);
    }

    /// <summary>
    /// Constructs a consumable object.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static ConsumableObject Construct(Inventory.ConsumableEntry entry)
    {
      var gameObj = new GameObject();
      gameObj.name = entry.Name;
      var itemObj = gameObj.AddComponent<ConsumableObject>();
      itemObj.Item = entry;
      return itemObj;
    }
  }

}