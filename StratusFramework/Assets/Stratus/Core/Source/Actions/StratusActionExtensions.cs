using UnityEngine;

  public static class StratusActionExtensions
  {

    public static Stratus.StratusActionsOwner Actions(this GameObject gameObj)
    {
      // Subscribe this GameObject to the ActionSpace
      var owner = Stratus.StratusActionSpace.Subscribe(gameObj);
      return owner;
    }

  }



