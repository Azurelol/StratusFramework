using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  [Singleton(instantiate = true)]
  public class ManagedBehaviourSystem : Singleton<ManagedBehaviourSystem>
  {
    private static List<ManagedBehaviour> behaviours = new List<ManagedBehaviour>();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
      Instantiate();
      ManagedBehaviour[] behaviours = FindObjectsOfType<ManagedBehaviour>();
      Trace.Script($"Adding {behaviours.Length} behaviours");
      behaviours.AddRange(behaviours);
    }

    protected override void OnAwake()
    {
      foreach (var behaviour in behaviours)
        behaviour.OnAwake();
    }

    private void Start()
    {
      foreach (var behaviour in behaviours)
        behaviour.OnStart();
    }

    private void Update()
    {
      foreach (var behaviour in behaviours)
        behaviour.OnUpdate();
    }

    private void FixedUpdate()
    {
      foreach (var behaviour in behaviours)
        behaviour.OnFixedUpdate();
    }

    private void LateUpdate()
    {
      foreach (var behaviour in behaviours)
        behaviour.OnLateUpdate();
    }

    public static void Add(ManagedBehaviour behaviour)
    {
      behaviours.Add(behaviour);
    }

    public static void Remove(ManagedBehaviour behaviour)
    {
      behaviours.Remove(behaviour);
    }


  }

}