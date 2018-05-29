using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rotorz.ReorderableList;
using UnityEditor;

namespace Stratus
{
  public class TriggerablesList : ReorderableListControl
  {
    private static readonly GUIContent menuItem1 = new GUIContent("Boop");
    public System.Action<int> onItemRemoved { get; set; }

    protected override void OnItemRemoving(ItemRemovingEventArgs args)
    {
      base.OnItemRemoving(args);
      onItemRemoved(args.ItemIndex);
    }

    protected override void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListAdaptor adaptor)
    {
      menu.AddItem(menuItem1, false, DefaultContextHandler, menuItem1);
    }

    protected override bool HandleCommand(string commandName, int itemIndex, IReorderableListAdaptor adaptor)
    {
      if (base.HandleCommand(commandName, itemIndex, adaptor))
        return true;

      switch (commandName)
      {
        case "Boop":
          Trace.Script("Boop");
          return true;
      }

      return false;
    }
  }

  public class TriggersList : ReorderableListControl
  {
    private static readonly GUIContent menuItem1 = new GUIContent("Boop");

    protected override void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListAdaptor adaptor)
    {
      menu.AddItem(menuItem1, false, DefaultContextHandler, menuItem1);
    }

    protected override bool HandleCommand(string commandName, int itemIndex, IReorderableListAdaptor adaptor)
    {
      if (base.HandleCommand(commandName, itemIndex, adaptor))
        return true;

      switch (commandName)
      {
        case "Boop":
          Trace.Script("Boop");
          return true;
      }

      return false;
    }
  }

}