/******************************************************************************/
/*!
@file   SpaceExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

/**************************************************************************/
/*!
@class SpaceExtensions 
*/
/**************************************************************************/
public static class SpaceExtensions {

  public static Stratus.Space Space(this GameObject gameObj)
  {
    return Stratus.Space.getSpace(gameObj.scene);
  }

  public static Stratus.Space Space(this MonoBehaviour component)
  {
    return Stratus.Space.getSpace(component.gameObject.scene);
  }

  public static GameSession GameSession(this GameObject gameObj)
  {
    return Stratus.GameSession.Instance;
  }

  public static GameSession GameSession(this MonoBehaviour gameObj)
  {
    return Stratus.GameSession.Instance;
  }


}
