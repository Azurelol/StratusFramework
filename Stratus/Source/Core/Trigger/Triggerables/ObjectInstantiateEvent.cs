/******************************************************************************/
/*!
@file   ObjectInstantiateDispatcher.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  /// <summary>
  /// Instantiates a prefab when triggered
  /// </summary>
  public class ObjectInstantiateEvent : Triggerable
  {
    public enum InstantiateProcedure { Parent, Replace, Unparent }
    public InstantiateProcedure Type = InstantiateProcedure.Parent;
    public GameObject Prefab;

    protected override void OnTrigger()
    {
      var obj = Instantiate(Prefab) as GameObject;
      obj.name.Replace("(Clone)", "");
      if (Type == InstantiateProcedure.Parent)
      {
        obj.transform.SetParent(this.transform);
      }
      else if (Type == InstantiateProcedure.Unparent)
      {
        obj.transform.SetParent(null);
        Destroy(this.gameObject);
      }
      else if (Type == InstantiateProcedure.Replace)
      {
        var parent = transform.parent;
        obj.transform.SetParent(parent, false);
        Destroy(this.gameObject);
      }     

    }

    protected override void OnAwake()
    {

    }


  }

}