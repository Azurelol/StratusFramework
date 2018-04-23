/******************************************************************************/
/*!
@file   ActionExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

  public static class ActionExtensions
  {

    public static Stratus.ActionsOwner Actions(this GameObject gameObj)
    {
      // Subscribe this GameObject to the ActionSpace
      var owner = Stratus.ActionSpace.Subscribe(gameObj);
      return owner;
    }

  }



