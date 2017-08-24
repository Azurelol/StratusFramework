/******************************************************************************/
/*!
@file   LinkController.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// The manager for all links that the overlying LinkInterface has access to.
    /// </summary>
    public class LinkController : MonoBehaviour
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The common style used by all the links managed by this controller
      /// </summary>
      [Tooltip("The common style used by all the links managed by this controller")]
      public Link.LinkStyle Style;
      /// <summary>
      /// The default layer of links for this controller
      /// </summary>
      [Tooltip("The default layer of links for this controller")]
      public LinkLayer DefaultLayer;
      /// <summary>
      /// The currently selected layer
      /// </summary>
      [Tooltip("The currently selected layer")]
      [ReadOnly] public LinkLayer CurrentLayer;
      /// <summary>
      /// The previously selected layer
      /// </summary>
      [Tooltip("The previously selected layer")]
      [ReadOnly] public LinkLayer PreviousLayer;
      /// <summary>
      /// Whether to pring debug output
      /// </summary>
      [Tooltip("Whether to pring debug output")]
      public bool Tracing = false;

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// All the links belonging to this controller
      /// </summary>
      [HideInInspector]
      public List<Link> Links = new List<Link>();
      /// <summary>
      /// All the layers belonging to this controller
      /// </summary>
      public Dictionary<string, LinkLayer> Layers = new Dictionary<string, LinkLayer>();
      /// <summary>
      /// The interface this controller belongs to
      /// </summary>
      LinkInterface Interface;

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes the LinkController script.
      /// </summary>
      void Awake()
      {
      }

      /// <summary>
      /// Connects the controller with the Link Interface.
      /// </summary>
      /// <param name="linkInterface"></param>
      public void Connect(LinkInterface linkInterface)
      {
        if (Tracing) Trace.Script("Connected to '" + linkInterface.gameObject.name + "'", this);
        Interface = linkInterface;
        Connect();
      }

      /// <summary>
      /// Connects all layers and links.
      /// </summary>
      public void Connect()
      {
        ConnectLinks();
        ConnectLayers();
      }

      /// <summary>
      /// Switches to the specified layer.
      /// </summary>
      /// <param name="layer"></param>
      public bool SwitchLayer(LinkLayer layer)
      {
        // If the layer does not exist...
        if (!layer)
          throw new Exception("No layer was passed!");

        return SwitchLayer(layer.name);
      }

      /// <summary>
      /// Switches to the specified layer.
      /// </summary>
      /// <param name="layerName">The name of the layer to switch to.</param>
      public bool SwitchLayer(string layerName)
      {
        // If there's no layer, do nothing
        if (Layers.Count == 0)
        {
          Trace.Script("No layers available!", this);
          return false;
        }

        // If the layer could not be found, do nothing
        if (!Layers.ContainsKey(layerName))
        {
          Trace.Script("The layer '" + layerName + "' could not be found!", this);
          return false;
        }

        // If it's the same layer, do nothing
        if (CurrentLayer.name == layerName)
        {
          if (Tracing) Trace.Script(layerName + " is the same layer!", this);
          return false;
        }

        // Save the previous layer
        PreviousLayer = CurrentLayer;
        // Disable the previous layer
        CurrentLayer.Enable(false);
        // Change the layer
        CurrentLayer = Layers[layerName];
        CurrentLayer.Enable(true);
        //Trace.Script("Switched to " + layerName, this);
        return true;
      }

      /// <summary>
      /// Switches back to the previous layer.
      /// </summary>
      /// <returns></returns>
      public bool SwitchLayerToPrevious()
      {
        //Trace.Script("Going back to ")
        return SwitchLayer(this.PreviousLayer);
      }

      /// <summary>
      /// Registers all links under this controller, including those parented to
      /// LinkLayers.
      /// </summary>
      void ConnectLinks()
      {
        // Remove any previous links
        Links.Clear();
        // Add all available links
        Links.AddRange(this.GetComponentsInChildren<Link>(true));
        if (Tracing) Trace.Script("Connecting '" + Links.Count + "' links!" , this);
        foreach (var link in Links)
        {
          link.Interface = Interface;
          // Override the link's style
          link.Style = this.Style;
          // Add an animation controller if its missing one?

          // Assign its neighbors
          if (link.Navigation.Mode == Link.LinkNavigation.NavigationMode.Automatic)
            link.Initialize();
            //link.AssignNeighbors();
        }
      }

      public void ApplyStyles()
      {
        var links = this.GetComponentsInChildren<Link>(true);
        foreach (var link in links)
        {
          // If it's missing a link animation component, add it!
          if (!link.gameObject.HasComponent<LinkAnimator>())
            link.gameObject.AddComponent<LinkAnimator>();
         
          // Set the default style 
          link.Style = this.Style;
          if (link.Style.Font != null) link.Text.font = link.Style.Font;
          link.Enable(link.Enabled);
          // Set its animator
          var animator = link.gameObject.GetComponent<LinkAnimator>();
          animator.SetAnimatorController();

        }
      }

      /// <summary>
      /// Registers all link layers under this controller, using a string dictionary
      /// to later access them.
      /// </summary>
      void ConnectLayers()
      {
        Layers.Clear();
        var layers = GetComponentsInChildren<LinkLayer>();
        foreach (var layer in layers)
        {
          layer.AddLinksFromChildren();
          Layers.Add(layer.name, layer);
        }
        // Use the first layer as the root layer
        if (layers.Length > 0)
          CurrentLayer = layers[0];

      }


      /// <summary>
      /// Selects the first link among available links.
      /// </summary>
      /// <returns></returns>
      public Link SelectFirstLink()
      {
        Link[] links;

        // Select the first link in the layer
        if (CurrentLayer)
        {
          //Trace.Script("Links available for the layer '" + CurrentLayer.name + "' = " + CurrentLayer.Links.Length, this);
          links = CurrentLayer.activeLinks;
        }
        // Otherwise use the common link pool
        else
        {
          links = Links.ToArray();
        }

        if (Links.Count > 0)
        {
          // Select the first link which is not hidden
          foreach (var link in links)
          {
            if (link.Hidden)
              continue;

            return link;
          }
        }

        // No link available!
        return null;
      }


      /// <summary>
      /// Sets the default style on all registered links.
      /// </summary>
      void SetDefaultStyle()
      {
        foreach (var link in Links)
        {
          link.Style = this.Style;
        }
      }

      /// <summary>
      /// Enables or disables all registered links.
      /// </summary>
      /// <param name="enabled">Whether the link should be enabled or not.</param>
      /// <param name="immediate">Whether this change should be done immediately.</param>
      public void Enable(bool enabled, bool immediate = false)
      {
        foreach (var link in Links)
        {
          link.Show(enabled, immediate);
        }
      }

      /// <summary>
      /// Finds a specified link by name.
      /// </summary>
      /// <param name="name">The name of the GameObject with the link.</param>
      /// <returns></returns>
      public Link Find(string name)
      {
        //var linkToFind = (from Link link in Links where link.name == name select link);
        var linkToFind = Links.Find(x => x.name == name);
        if (!linkToFind)
        {
          Trace.Script("Could not find link of name '" + name + "'", this);
          Trace.Script("Available links: ", this);
          foreach (var link in Links) Trace.Script("- " + link.name, this);
          throw new System.Exception();
        }

        return linkToFind;
      }

      /// <summary>
      /// Deselects all links
      /// </summary>
      public void DeselectAll()
      {
        // wat
      }

    }

  } 
}