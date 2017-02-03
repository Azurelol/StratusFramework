/******************************************************************************/
/*!
@file   LinkLayer.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// A group of links, so that they can only navigate with those within the layer.
    /// Also allows them to be shown/hidden at once.
    /// </summary>
    public class LinkLayer : StratusBehaviour
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The links currently attached to this layer.
      /// </summary>
      [HideInInspector] public Link[] Links;
      [ReadOnly] public Link CurrentLink;
      public Link First { get { return Links[0]; } }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes the LinkLayer
      /// </summary>
      void Awake()
      {
        //RegisterLinks();
      }

      /// <summary>
      /// Registers all links under this layer.
      /// </summary>
      public void RegisterLinks()
      {
        //Trace.Script("Registering links to this layer", this);
        Links = this.GetComponentsInChildren<Link>(true);
      }

      /// <summary>
      /// Restores the state of this layer.
      /// </summary>
      public Link Restore()
      {
        return CurrentLink;
      }

      /// <summary>
      /// Resets the state of this layer to default. This will deactivate all links.
      /// </summary>
      public void Reset()
      {
        foreach (var link in Links)
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
        foreach (var link in Links)
        {
          link.Show(enabled, immediate);
          //link.gameObject.SetActive(enabled);
        }
      }

    }

  } 
}