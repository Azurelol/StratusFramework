/******************************************************************************/
/*!
@file   LinkLayer.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// A group of links, so that they can only navigate with those within the layer.
    /// Also allows them to be shown/hidden at once.
    /// </summary>
    public class LinkLayer : MonoBehaviour
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The links currently attached to this layer.
      /// </summary>
      public Link[] activeLinks => links.ToArray();
      /// <summary>
      /// The currently active link
      /// </summary>
      public Link currentLink { get; set; }
      /// <summary>
      /// The first link
      /// </summary>
      public Link first { get { return links[0]; } }

      private List<Link> links = new List<Link>();

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Registers all links under this layer.
      /// </summary>
      public void AddLinksFromChildren()
      {
        Clear();
        links.AddRange(this.GetComponentsInChildren<Link>(true));
      }

      /// <summary>
      /// Clears all links
      /// </summary>
      public void Clear()
      {
        links.Clear();
      }

      /// <summary>
      /// Restores the state of this layer.
      /// </summary>
      public Link Restore()
      {
        return currentLink;
      }

      /// <summary>
      /// Resets the state of this layer to default. This will deactivate all links.
      /// </summary>
      public void Reset()
      {
        foreach (var link in activeLinks)
        {
          link.Deactivate();
          link.Deselect();
        }
      }

      /// <summary>
      /// Enables or disables all registered links.
      /// </summary>
      /// <param name="enabled">Whether the link should be enabled or not.</param>
      public void Enable(bool enabled, bool immediate = false)
      {
        foreach (var link in activeLinks)
        {
          link.Show(enabled, immediate);
          //link.gameObject.SetActive(enabled);
        }
      }

    }

  } 
}