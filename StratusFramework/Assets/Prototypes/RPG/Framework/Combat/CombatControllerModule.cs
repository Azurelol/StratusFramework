using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
  public abstract class CombatControllerModule
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public CombatController controller { get; private set; }
    public GameObject gameObject { get; private set; }

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void OnInitialize();
    protected abstract void OnSubscribe();
    public abstract void OnTimeStep(float step);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public void Initialize(CombatController controller)
    {
      this.controller = controller;
      this.gameObject = controller.gameObject;
      this.OnSubscribe();
      this.OnInitialize();
    }
  }

  public abstract class CombatControllerAttributeModule<T> : CombatControllerModule where T : Combat.Attribute
  {
  }

}