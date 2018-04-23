/******************************************************************************/
/*!
@file   Agent_Debug.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.AI;

namespace Stratus
{
  namespace AI
  {
    public abstract partial class Agent : StratusBehaviour
    {
      private void OnDrawGizmos()
      {
        DrawPathByHandle();
      }

      void DrawPathByHandle()
      {
        if (navigation == null || navigation.path == null)
          return;

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawAAPolyLine(navigation.path.corners);
#endif
      }

      private void Debug()
      {
        if (!navigation.hasPath)
        {
          this.lineRenderer.enabled = false;
          return;
        }
        this.lineRenderer.enabled = true;
        this.DrawPath();
      }

      void AddLineRenderer()
      {
        this.lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        this.lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
        this.lineRenderer.startWidth = 0.1f;
        this.lineRenderer.endWidth = 0.1f;
        this.lineRenderer.startColor = Color.yellow;
        this.lineRenderer.endColor = Color.yellow;
      }

      /// <summary>
      /// Draws a path to the target using the line renderer
      /// </summary>
      void DrawPath()
      {
        if (this.lineRenderer == null)
        {
          this.AddLineRenderer();
        }

        this.lineRenderer.positionCount = navigation.path.corners.Length;
        for (int i = 0; i < navigation.path.corners.Length; i++)
        {
          var corner = navigation.path.corners[i];
          UnityEngine.Debug.DrawRay(corner, Vector3.up, Color.red);
          this.lineRenderer.SetPosition(i, navigation.path.corners[i]);
        }
      }

      void PrintPath(NavMeshPath path)
      {
        var pathStr = "Path:";
        foreach (var corner in path.corners)
        {
          pathStr += " " + corner;
        }
        Trace.Script(pathStr, this);
      }


    }
  }

}