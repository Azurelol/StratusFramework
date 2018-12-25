using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Stratus
{
  namespace AI
  {
    public partial class Agent : ManagedBehaviour
    {
      /// <summary>
      /// Used to temporarily set, then revert navigation settings
      /// </summary>
      public struct NavigationSettings
      {
        private NavMeshAgent navigation;
        private float previousSpeed;
        private float previousAcceleration;
        private float previousStoppingDistance;

        public NavigationSettings(NavMeshAgent agent)
        {
          navigation = agent;
          previousSpeed = navigation.speed;
          previousAcceleration = navigation.acceleration;
          previousStoppingDistance = navigation.stoppingDistance;
        }

        public void Set(float speed, float acceleration, float stoppingDistance)
        {
          this.navigation.speed = speed;
          this.navigation.acceleration = acceleration;
          this.navigation.stoppingDistance = stoppingDistance;
        }

        public void Revert()
        {
          this.navigation.speed = previousSpeed;
          this.navigation.acceleration = previousAcceleration;
          this.navigation.stoppingDistance = previousStoppingDistance;
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
        this.OnAgentMovementStarted();

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

        this.OnAgentMovementEnded();        
      }


      /// <summary>
      /// Moves this agent through a list of points.
      /// </summary>
      protected IEnumerator FollowPathRoutine(Vector3[] points, float speed, float acceleration, float stoppingDistance)
      {
        // Store, then set the new settings
        var navSettings = new NavigationSettings(this.navigation);
        navSettings.Set(speed, acceleration, stoppingDistance);

        this.OnAgentMovementStarted();

        // Create a new line renderer to draw this path
        //PathDisplay pathDisplay;
        //if (this.IsDebugging)
        //{
        //  pathDisplay = new PathDisplay(this.gameObject, points, Color.red, Color.blue);
        //}

        IEnumerator drawRoutine = null;
        if (this.debug)
        {
          drawRoutine = DrawPathRoutine(points, Color.red, Color.yellow);
          StartCoroutine(drawRoutine);
        }

        // Now travel the path
        foreach (var point in points)
        {
          StratusDebug.Log("Moving to next point: " + point, this);
          while (Vector3.Distance(transform.position, point) > stoppingDistance)
          {
            this.navigation.SetDestination(point);
            yield return new WaitForFixedUpdate();
          }
        }

        StratusDebug.Log("Finished the path!");
        if (this.debug)
        {
          StopCoroutine(drawRoutine);
        }


        this.OnAgentMovementEnded();
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