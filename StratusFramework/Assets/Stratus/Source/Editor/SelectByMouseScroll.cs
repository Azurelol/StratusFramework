/******************************************************************************/
/*!
@file   SelectByMouseScroll.cs
@author Fredrik Ludvigsen
*/
/******************************************************************************/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Stratus
{
  [InitializeOnLoad]
  public static class SelectByMouseScroll
  {
    private static Renderer[] renderers;
    private static int index;

    static SelectByMouseScroll()
    {
      HierarchyUpdate();
      EditorApplication.hierarchyWindowChanged += HierarchyUpdate;
      SceneView.onSceneGUIDelegate += HighlightUpdate;
    }

    private static void HierarchyUpdate() { renderers = Object.FindObjectsOfType<Renderer>(); }

    private static void HighlightUpdate(SceneView sceneview)
    {
      if (UnityEngine.Event.current.type != EventType.ScrollWheel || !UnityEngine.Event.current.alt)
        return;
      var mp = UnityEngine.Event.current.mousePosition;
      var ccam = Camera.current;
      var mouseRay = ccam.ScreenPointToRay(new Vector3(mp.x, ccam.pixelHeight - mp.y, 0f));

      index += UnityEngine.Event.current.delta.y >= 0f ? -1 : 1;

      var pointedRenderers = new List<Renderer>();
      foreach (Renderer r in renderers)
        if (r.bounds.IntersectRay(mouseRay))
          pointedRenderers.Add(r);

      if (pointedRenderers.Count > 0)
      {
        index = (index + pointedRenderers.Count) % pointedRenderers.Count;
        Selection.objects = new Object[] { pointedRenderers[index].gameObject };
        UnityEngine.Event.current.Use();
      }
    }

  }
}

