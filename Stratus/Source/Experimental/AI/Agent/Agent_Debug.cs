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
    public abstract partial class Agent : MonoBehaviour
    {
      private void OnDrawGizmos()
      {
      }

      private void Debug()
      {
        if (!Navigation.hasPath)
        {
          this.LineRenderer.enabled = false;
          return;
        }
        this.LineRenderer.enabled = true;
        this.DrawPath();
      }

      void AddLineRenderer()
      {
        this.LineRenderer = this.gameObject.AddComponent<LineRenderer>();
        this.LineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
        this.LineRenderer.startWidth = 0.1f;
        this.LineRenderer.endWidth = 0.1f;
        this.LineRenderer.startColor = Color.yellow;
        this.LineRenderer.endColor = Color.yellow;
      }

      /// <summary>
      /// Draws a path to the target using the line renderer
      /// </summary>
      void DrawPath()
      {
        if (this.LineRenderer == null)
        {
          this.AddLineRenderer();
        }

        this.LineRenderer.positionCount = Navigation.path.corners.Length;
        for (int i = 0; i < Navigation.path.corners.Length; i++)
        {
          var corner = Navigation.path.corners[i];
          UnityEngine.Debug.DrawRay(corner, Vector3.up, Color.red);
          this.LineRenderer.SetPosition(i, Navigation.path.corners[i]);
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