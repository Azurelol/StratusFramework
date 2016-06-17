using UnityEngine;
using System.Collections;

  public static class ActionExtensions
  {

    public static Stratus.ActionsOwner Actions(this GameObject gameObj)
    {
      // Subscribe this GameObject to the ActionSpace
      var owner = Stratus.ActionSpace.Subscribe(gameObj);
      return owner;
    }

  }



