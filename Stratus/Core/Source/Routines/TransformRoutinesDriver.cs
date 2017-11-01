using UnityEngine;
using System.Collections;
using System;

namespace Stratus
{
  /// <summary>
  /// The type of transformation this routine performs
  /// </summary>
  [Flags]
  public enum TransformationType
  {
    Translate = 1,
    Rotate = 2,
    Scale = 4
  }

  /// <summary>
  /// A class encapsulating a transform routine
  /// </summary>
  public abstract class TransformRoutine
  {
    protected Transform transform;
    protected System.Action onFinished;
    public abstract TransformationType type { get; }

    protected abstract IEnumerator OnTransform();

    public IEnumerator Bind(Transform transform, System.Action onFinished = null)
    {
      this.transform = transform;
      this.onFinished = onFinished;
      return OnTransform();
    }
  }

  public class TransformRoutineBinder
  {
    public TransformRoutineBinder(IEnumerator routine, TransformationType type)
    {
      this.routine = routine;
      this.type = type;
    }

    public IEnumerator routine { get; set; }
    public TransformationType type { get; set; }
  }

  /// <summary>
  /// This component handles the management of running transform routines within the same GameObject.
  /// It will make sure there's no overlap between routines of the same kind (translation, scaling, rotation).
  /// </summary>
  [DisallowMultipleComponent]
  public class TransformRoutinesDriver : MonoBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Classes
    //--------------------------------------------------------------------------------------------/
    private class RoutineActivity
    {
      public IEnumerator routine;
      public bool isActive;
    }

    //--------------------------------------------------------------------------------------------/
    // Fieldss
    //--------------------------------------------------------------------------------------------/
    private RoutineActivity translation = new RoutineActivity();
    private RoutineActivity rotation = new RoutineActivity();
    private RoutineActivity scaling = new RoutineActivity();

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public void Translate(IEnumerator routine) => StartRoutine(routine, translation);
    public void Rotate(IEnumerator routine) => StartRoutine(routine, rotation);
    public void Scale(IEnumerator routine) => StartRoutine(routine, scaling);

    public void StopTranslation() => StopRoutine(translation);
    public void StopRotation() => StopRoutine(rotation);
    public void StopScaling() => StopRoutine(scaling);
    
    public void StartTransformation(IEnumerator routine, TransformationType type, System.Action onFinished = null)
    {
      RoutineActivity transformation = null;

      bool isTranslation = type.HasFlag(TransformationType.Translate);
      bool isRotation = type.HasFlag(TransformationType.Rotate);
      bool isScaling = type.HasFlag(TransformationType.Scale);

      // For readability: if need better perf, just move to the branches?
      if (isTranslation) StopTranslation();
      if (isRotation) StopRotation();
      if (isScaling) StopScaling();
      
      if (isTranslation)
      {
        StopTranslation();
        transformation = translation;
        if (isRotation) rotation = transformation;
        if (isScaling) scaling = transformation;
      }
      else if (isRotation)
      {
        transformation = rotation;
        if (isScaling) scaling = transformation;
      }
      else
      {
        transformation = scaling;
      }
      
      StartRoutine(routine, transformation, onFinished);
    }

    public void StopTransformation(TransformationType type)
    {
      bool isTranslation = type.HasFlag(TransformationType.Translate);
      bool isRotation = type.HasFlag(TransformationType.Rotate);
      bool isScaling = type.HasFlag(TransformationType.Scale);
      
      if (isTranslation) StopTranslation();
      if (isRotation) StopRotation();
      if (isScaling) StopScaling();
    }

    private void StartRoutine(IEnumerator newRoutine, RoutineActivity transformation, System.Action onFinished = null)
    {
      StopRoutine(transformation);

      if (newRoutine == null)
        return;

      transformation.routine = newRoutine;

      // If there's a callback to be called when this routine stops
      if (onFinished != null)
      {        
        StartCoroutine(StartRoutineWithCallback(newRoutine, onFinished));
      }
      // Otherwise just start it
      else
      {
        StartCoroutine(newRoutine);
      }
      transformation.isActive = true;
    }

    private void StopRoutine(RoutineActivity transformation)
    {
      if (transformation.routine != null)
        StopCoroutine(transformation.routine);
      transformation.isActive = false;
    }

    private IEnumerator StartRoutineWithCallback(IEnumerator routine, System.Action onFinished)
    {
      yield return routine;
      onFinished.Invoke();
    }
  }
}