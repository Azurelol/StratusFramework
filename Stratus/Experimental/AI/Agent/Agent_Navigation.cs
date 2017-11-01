/******************************************************************************/
/*!
@file   Agent_Navigation.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Stratus
{
  namespace AI
  {
    public abstract partial class Agent : MonoBehaviour
    {
      /// <summary>
      /// Used to temporarily set, then revert navigation settings
      /// </summary>
      public struct NavigationSettings
      {
        private NavMeshAgent Navigation;
        private float PreviousSpeed;
        private float PreviousAcceleration;
        private float PreviousStoppingDistance;

        public NavigationSettings(NavMeshAgent agent)
        {
          Navigation = agent;
          PreviousSpeed = Navigation.speed;
          PreviousAcceleration = Navigation.acceleration;
          PreviousStoppingDistance = Navigation.stoppingDistance;
        }

        public void Set(float speed, float acceleration, float stoppingDistance)
        {
          this.Navigation.speed = speed;
          this.Navigation.acceleration = acceleration;
          this.Navigation.stoppingDistance = stoppingDistance;
        }

        public void Revert()
        {
          this.Navigation.speed = PreviousSpeed;
          this.Navigation.acceleration = PreviousAcceleration;
          this.Navigation.stoppingDistance = PreviousStoppingDistance;
        }
      }

      /// <summary>
      /// Displays a path during its lifetime using a line renderer
      /// </summary>
      public class PathDisplay
      {
        LineRenderer Renderer;
        public PathDisplay(GameObject owner, Vector3[] points, Color starting, Color ending)
        {
          //this.Renderer = new LineRenderer();
          this.Renderer = owner.AddComponent<LineRenderer>();
          this.Renderer.positionCount = points.Length;
          this.Renderer.startWidth = 0.1f;
          this.Renderer.endWidth = 0.1f;
          this.Renderer.SetPositions(points);
          this.Renderer.startColor = starting;
          this.Renderer.endColor = ending;
        }

        ~PathDisplay()
        {
          Destroy(this.Renderer);
        }
      }

      //void ApproachTarget

      /// <summary>
      /// Attempts to approach the target until it gets within range
      /// </summary>
      IEnumerator ApproachTargetRoutine(Transform target, float speed, float acceleration, float stoppingDistance, float angle)
      {
        // Store, then set the new settings
        var navSettings = new NavigationSettings(this.navigation);
        navSettings.Set(speed, acceleration, stoppingDistance);

        //Trace.Script("Will now approach " + target.name + " up until " + stoppingDistance + " units at " + speed + " speed!", this);
        this.OnMovementStarted();

        // While we are not within range of the target, keep making paths to it
        while (Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
          //Trace.Script("Current distance = " + dist);
          this.MoveTo(target.transform.position);

          yield return new WaitForFixedUpdate();
        }

        //Trace.Script("Approached " + target.name, this);

        // Now that we are in range of the target, let's revert to the old settings and stop moving
        this.navigation.isStopped = true;
        navSettings.Revert();
        this.steeringRoutine = null;

        this.OnMovementEnded();        
      }

      //protected void FollowPath(Vector3[] points, float speed, float acceleration, float stoppingDistance)
      //{
      //  if (CurrentRoutine != null) StopCoroutine(CurrentRoutine);
      //  CurrentRoutine = FollowPathRoutine(points, speed, acceleration, stoppingDistance);
      //  StartCoroutine(CurrentRoutine);
      //}

      /// <summary>
      /// Moves this agent through a list of points.
      /// </summary>
      protected IEnumerator FollowPathRoutine(Vector3[] points, float speed, float acceleration, float stoppingDistance)
      {
        // Store, then set the new settings
        var navSettings = new NavigationSettings(this.navigation);
        navSettings.Set(speed, acceleration, stoppingDistance);

        this.OnMovementStarted();

        // Create a new line renderer to draw this path
        //PathDisplay pathDisplay;
        //if (this.IsDebugging)
        //{
        //  pathDisplay = new PathDisplay(this.gameObject, points, Color.red, Color.blue);
        //}

        IEnumerator drawRoutine = null;
        if (this.logging)
        {
          drawRoutine = DrawPathRoutine(points, Color.red, Color.yellow);
          StartCoroutine(drawRoutine);
        }

        // Now travel the path
        foreach (var point in points)
        {
          Trace.Script("Moving to next point: " + point, this);
          while (Vector3.Distance(transform.position, point) > stoppingDistance)
          {
            this.navigation.SetDestination(point);
            yield return new WaitForFixedUpdate();
          }
        }

        Trace.Script("Finished the path!");
        if (this.logging)
        {
          StopCoroutine(drawRoutine);
        }


        this.OnMovementEnded();
        this.navigation.isStopped = true;
        navSettings.Revert();



      }

      IEnumerator DrawPathRoutine(Vector3[] points, Color starting, Color ending)
      {
        while (true)
        {
          //Trace.Script("Drawing points!");
          foreach (var point in points)
          {
            UnityEngine.Debug.DrawRay(point, Vector3.up, Color.green);
          }
          yield return new WaitForFixedUpdate();
        }
      }




    }
  } 
}