using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stratus
{
  namespace Gameplay
  { 
    public class TopDownCameraController : CameraControllerBase
    { 
      [Header("Displacement")]
      [Tooltip("How many units to pan per second")]
      public float panSpeed = 20f;
      [Tooltip("How many pixels within the edge of the screen must the mouse be in order to pan the camera along that direction")]
      public float panBorderThickness = 10f;

      public float scrollSpeed = 20f;


      [Header("Boundaries")]
      public bool hasBoundary = true;
      public Vector3 boundary = new Vector3(50f, 40f, 50f);
      public float maximumDistanceFromNavMesh = 5f;

      [Header("Input")]
      public InputAxisField horizontalAxis = new InputAxisField();
      public InputAxisField verticalAxis = new InputAxisField();
      public InputAxisField scrollAxis = new InputAxisField();

      protected override void OnAwake()
      {
      }

      protected override void OnUpdate()
      {
        Vector3 pos = transform.position;
        float displacement = panSpeed * Time.deltaTime;

        // Screen origin is at the bottom (0,0)
        bool useMouse = IsMouseWithinGameWindow();
        bool isCursorOnTop = useMouse && Input.mousePosition.y >= Screen.height - panBorderThickness;
        bool isCursorOnBottom = useMouse && Input.mousePosition.y <= panBorderThickness;
        bool isCursorOnRight = useMouse &&  Input.mousePosition.x >= Screen.width - panBorderThickness;
        bool isCursorOnLeft = useMouse && Input.mousePosition.x <= panBorderThickness;        

        if (verticalAxis.isPositive || isCursorOnTop) pos.z += displacement;
        if (verticalAxis.isNegative || isCursorOnBottom) pos.z -= displacement;
        if (horizontalAxis.isPositive || isCursorOnRight) pos.x += displacement;
        if (horizontalAxis.isNegative || isCursorOnLeft) pos.x -= displacement;

        // Scroll
        float scroll = scrollAxis.value * scrollSpeed * 100f *  Time.deltaTime;
        pos.y -= scroll;

        if (hasBoundary)
        {
          pos.x = Mathf.Clamp(pos.x, -boundary.x, boundary.x);
          pos.y = Mathf.Clamp(pos.y, 5f, boundary.y);
          pos.z = Mathf.Clamp(pos.z, -boundary.z, boundary.z);

          // if (IsWithinNavMesh(pos, maximumDistanceFromNavMesh) == false)
          //  return;
        }


        transform.position = Vector3.Lerp(transform.position, pos, 100f * Time.deltaTime);
        
      }

      bool IsWithinNavMesh(Vector3 position, float maxDistance)
      {        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position.StripComponent(StructExtensions.Vector3Component.y), out hit, maxDistance, NavMesh.AllAreas))
          return true;
        return false;
      }

      bool IsMouseWithinGameWindow()
      {
        Rect screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
        if (screenRect.Contains(Input.mousePosition))
          return true;
        return false;
      }


    }

  }
}