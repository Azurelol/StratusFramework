/******************************************************************************/
/*!
@file   ScrollingLinks.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Provides a scrolling list interface for links, using Unity's Canvas UI.
    /// </summary>
    //[RequireComponent(typeof(LinkController))]
    public class ScrollingLinks : MonoBehaviour
    {
      //public LinkController Controller { get { return GetComponent<LinkController>(); } }
      [Tooltip("Prefab used for the scrolling links")]
      public GameObject Prefab;
      public List<Link> Links = new List<Link>();

      void Awake()
      {

      }

      /// <summary>
      /// Adds a link and positions it dynamically.
      /// </summary>
      /// <typeparam name="T">The type of link to add</typeparam>
      public T Add<T>(string objectName, string text) where T : Link
      {
        //Trace.Script("Adding '" + objectName + "'", this);
        // Add the link to the prefab, parenting to this controller
        var linkPrefab = Instantiate(Prefab) as GameObject;
        linkPrefab.transform.SetParent(this.transform, false);
        var link = linkPrefab.AddComponent<T>();
        link.name = objectName;
        link.Text.text = text;
        // Add it to the list of scrolling links
        Links.Add(link);
        // Connnect again
        //Controller.Connect();
        return link;
      }



      /// <summary>
      /// Clears all scrolling links.
      /// </summary>
      public void Clear()
      {
        //Trace.Script("Clearing all scrolling links", this);
        foreach (var link in Links)
        {
          DestroyImmediate(link.gameObject);
        }
        Links.Clear();

        //var seq = Actions.Sequence(this);
        //Actions.Delay(seq, 0.1f);
        //Actions.Call(seq, this.PostClear);
      }

      void PostClear()
      {
        // Connect links again
        //Controller.Connect();
      }

    }

  } 
}