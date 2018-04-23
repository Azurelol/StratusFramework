using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stratus
{
  /// <summary>
  /// A display for singletons
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class SingletonSceneViewDisplay<T> : LayoutSceneViewDisplay where T : Singleton<T>
  {
    protected virtual bool showInPlayMode { get; } = true;
    protected T instance => Singleton<T>.get;
    protected override bool isValid => showInPlayMode && Singleton<T>.instantiated && instance.isActiveAndEnabled;
    protected abstract void OnInitializeSingletonState();
    

    protected SerializedPropertyMap properties { get; private set; }

    protected override void OnInitializeState()
    {
      if (instance)
        properties = new SerializedPropertyMap(instance);
      OnInitializeSingletonState();
    }

  } 
}
