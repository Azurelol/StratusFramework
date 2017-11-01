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

    [Header("Instantiate")]
    public InstantiateProcedure type = InstantiateProcedure.Parent;
    public GameObject prefab;

    [Header("Transform")]
    public Vector3 position = new Vector3();
    public bool isWorldSpace = false;
    protected override void OnAwake()
    {

    }

    protected override void OnTrigger()
    {
      var obj = Instantiate(prefab) as GameObject;

      // Remove the clone prefix
      obj.name.Replace("(Clone)", "");

      if (type == InstantiateProcedure.Parent)
      {
        obj.transform.SetParent(this.transform);        
      }
      else if (type == InstantiateProcedure.Unparent)
      {
        obj.transform.SetParent(null);
        Destroy(this.gameObject);
      }
      else if (type == InstantiateProcedure.Replace)
      {
        var parent = transform.parent;
        obj.transform.SetParent(parent, false);
        Destroy(this.gameObject);
      }

      obj.transform.localPosition = position;

    }



  }

}