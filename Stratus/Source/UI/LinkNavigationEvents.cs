/******************************************************************************/
/*!
@file   LinkNavigationEvents.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using UnityEngine.Events;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Used to specify navigational direction for links.
    /// </summary>
    [Serializable]
    public enum Navigation { Up, Down, Left, Right }

    /// <summary>
    /// What axis to use when navigating.
    /// </summary>
    public enum NavigationAxis { Horizontal, Vertical }


    /// <summary>
    /// Custom callback for a method taking a direction.
    /// </summary>
    [Serializable]
    public class NavigationCallback : UnityEvent<Navigation> { }

  } 
}