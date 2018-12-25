using UnityEngine;
using UnityEngine.AI;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Displays a path during its lifetime using a line renderer
    /// </summary>
    public class PathDisplay
    {
      LineRenderer renderer;

      public PathDisplay(GameObject owner, Vector3[] points, Color starting, Color ending)
      {
        //this.Renderer = new LineRenderer();
        this.renderer = owner.AddComponent<LineRenderer>();
        this.renderer.positionCount = points.Length;
        this.renderer.startWidth = 0.1f;
        this.renderer.endWidth = 0.1f;
        this.renderer.SetPositions(points);
        this.renderer.startColor = starting;
        this.renderer.endColor = ending;
      }

      ~PathDisplay()
      {
        Object.Destroy(this.renderer);
      }
    }

    /// <summary>
    /// Provides debugging tools and information for AI agents
    /// </summary>
    [RequireComponent(typeof(Agent))]
    public class AgentDebug : ManagedBehaviour
    {
      //------------------------------------------------------------------------/
      // Fields: Private
      //------------------------------------------------------------------------/

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      private Agent agent { get; set; }
      private NavMeshAgent navigation { get; set; }
      protected LineRenderer lineRenderer { get; set; }

      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      void Start()
      {
        agent = GetComponent<Agent>();
        navigation = GetComponent<NavMeshAgent>();
        StratusGUI.Watch(() => agent.currentState, "Behavior", this);
      }

      private void OnDrawGizmos()
      {
        DrawPathByHandle();
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
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
        StratusDebug.Log(pathStr, this);
      }



    }
  }

}