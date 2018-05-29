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
    public PositionField position = new PositionField();
    public UnityEngine.Space space = UnityEngine.Space.Self;

    public override string automaticDescription
    {
      get
      {
        if (prefab)
        {
          string value = $"Instantiate {prefab.name} at {position} ({space})";      
          return value;
        }
        return string.Empty;
      }
    }

    protected override void OnAwake()
    {
    }

    protected override void OnReset()
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

      if (space == UnityEngine.Space.Self)
        obj.transform.localPosition = position;
      else
        obj.transform.position = position;

    }



  }

}