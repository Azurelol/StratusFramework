using UnityEngine;
using Stratus;
using System.Collections;

namespace Stratus
{
  namespace Samples
  {
    public class RoutinesSample : MonoBehaviour
    {
      [Header("General Settings")]
      public Transform target;
      [Range(0f, 5f)]
      public float duration = 1f;

      [Header("Movement")]
      public float distance = 0f;
      public float moveSpeed = 1f;
      public bool maintainDistance = true;

      [Header("Rotation")]
      public float angleAroundTarget = 90f;
      public Vector3 axis = Vector3.up;
      public float damping = 1f;

      [Header("Scaling")]
      public float scalar = 3f;
      public bool repeat = false;
      public AnimationCurve scalingCurve = new AnimationCurve();
      
      public StratusRuntimeMethodField transformRoutines;
      public StratusRuntimeMethodField taggedRoutines;

      private void Start()
      {
        transformRoutines = new StratusRuntimeMethodField(new System.Action[]
        {
          StopAll,
          MoveTo,
          Follow,
          RotateAround,
          RotateAroundWhile,
          LookAt,
          Track,
          Scale,
        });

        taggedRoutines = new StratusRuntimeMethodField(new System.Action[]
        {
          ScaleSequence,
          StopScaleSequence
        });

      }

      //----------------------------------------------------------------------/
      // Methods: Routines
      //----------------------------------------------------------------------/

      void StopAll()
      {
        this.transform.StopCoroutine(TransformationType.Translate | TransformationType.Rotate | TransformationType.Scale);
      }
      void MoveTo()
      {
        IEnumerator move = StratusRoutines.MoveTo(transform, target.position, duration);
        this.transform.StartCoroutine(move, TransformationType.Translate);
      }
      void Follow()
      {
        IEnumerator follow = StratusRoutines.Follow(transform, target, moveSpeed, distance, maintainDistance);
        this.transform.StartCoroutine(follow, TransformationType.Translate);
      }
      void LookAt()
      {
        IEnumerator lookAt = StratusRoutines.LookAt(transform, target, duration);
        this.transform.StartCoroutine(lookAt, TransformationType.Rotate);
      }
      void RotateAround()
      {
        IEnumerator rotateAround = StratusRoutines.RotateAround(transform, target.position, axis, angleAroundTarget, duration);
        this.transform.StartCoroutine(rotateAround, TransformationType.Translate | TransformationType.Rotate, OnFinished);
      }
      void RotateAroundWhile()
      {
        IEnumerator rotateAround = StratusRoutines.RotateAround(transform, target.position, axis, angleAroundTarget);
        this.transform.StartCoroutine(rotateAround, TransformationType.Translate | TransformationType.Rotate);
      }
      void Track()
      {
        IEnumerator track = StratusRoutines.Track(transform, target, damping);
        this.transform.StartCoroutine(track, TransformationType.Rotate);
      }
      void Scale()
      {
        IEnumerator scale = StratusRoutines.Scale(transform, this.scalar, this.duration);
        this.transform.StartCoroutine(scale, TransformationType.Scale);
      }

      void ScaleSequence()
      {
        var scaling = StratusRoutines.Scale(transform, new float[] { 0.5f, 4f, 1f }, duration, repeat);
        this.StartCoroutine(scaling, "Scale");
        
      }

      void StopScaleSequence()
      {
        this.StopTaggedCoroutine("Scale");
      }

      void ScaleOnCurve()
      {
        float curveDur = this.scalingCurve.keys[this.scalingCurve.length - 1].time;
        IEnumerator scale = StratusRoutines.Scale(transform, this.scalingCurve, curveDur);
        this.transform.StartCoroutine(scale, TransformationType.Scale);
      }

      void OnFinished()
      {
        StratusDebug.Log("Routine has finished!");
      }

      

    } 
  } 
}
