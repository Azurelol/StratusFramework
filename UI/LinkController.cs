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
      public bool Active;
      public Link.LinkStyle Style;
      [HideInInspector]
      public List<Link> Links = new List<Link>();
      public Dictionary<string, LinkLayer> Layers = new Dictionary<string, LinkLayer>();
      [ReadOnly]
      public LinkLayer CurrentLayer;
      [ReadOnly]
      public LinkLayer PreviousLayer;
      //public bool Active { set { this.gameObject.SetActive(value); } }
      LinkInterface Interface;
      public bool Tracing = false;

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
          Trace.Script(layerName + " is the same layer!", this);
          return false;
        }

        // Save the previous layer
        PreviousLayer = CurrentLayer;
        // Disable the previous layer
        CurrentLayer.Enable(false);
        // Change the layer
        CurrentLayer = Layers[layerName];
        CurrentLayer.Enable(true);
        Trace.Script("Switched to " + layerName, this);
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
        var links = this.GetComponentsInChildren<Link>(true);
        Links.AddRange(links);
        //Trace.Script("Connecting '" + Links.Count + "' links:" + Links , this);
        foreach (var link in Links)
        {
          //if (Tracing)
          //Add(link);
          link.Interface = Interface;
          // Override the link's style
          link.Style = this.Style;
          // Assign its neighbors
          if (link.Navigation.Mode == Link.LinkNavigation.NavigationMode.Automatic)
            link.AssignNeighbors();
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
          links = CurrentLayer.Links;
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