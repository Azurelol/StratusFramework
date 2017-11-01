using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  namespace Utilities
  {
    class PhysicsHelper
    {
      //private void TestPosition(Transform transform, Collider other)
      //{
      //  RaycastHit hit;
      //  Physics.Raycast(transform.position, other.transform.position, out hit);
      //
      //  // Convert collision point to local space
      //  // Gives us a poitn relative to the box collider itself
      //  Vector3 localPoint = hit.transform.InverseTransformPoint(hit.point);
      //  Vector3 localDir = localPoint.normalized;
      //
      //  // Is the point above or below the box? To the right or left? In front of behind?
      //  // A dot product of -1 tells us the vectors are pointing in opposidre directions,
      //  // 0 if perpendicular, 1 if exactly parallel
      //  // Our point might be slightly above the box, and far to the right of it. 
      //  // These dot products tell us how far the point has moved in any of those three directions.
      //
      //  // If upDot is positive, we're above the box; if negative, we're below the box.
      //  float upDot = Vector3.Dot(localDir, Vector3.up);
      //  // If fwdDot is positive, we're in front of the box; if negative, we're behind the box.
      //  float fwDot = Vector3.Dot(localDir, Vector3.forward);
      //  // If rightDot is positive, we're to the box's right; if negative, we're to its left.
      //  float rightDot = Vector3.Dot(localDir, Vector3.right);
      //
      //  // If upPower is the largest, we are mostly above or below the box
      //  float upPower = Mathf.Abs(upDot);
      //  // If fwdPower is the largest, we are msotly in front or behind the box
      //  float fwdPower = Mathf.Abs(fwDot);
      //  // If rightPower is the largest, we are mostly to the left or right of the box
      //  float rightPower = Mathf.Abs(rightDot);
      //}
    }

  }

}