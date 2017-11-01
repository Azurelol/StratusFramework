using System;
using UnityEngine;
using Stratus.Dependencies.TypeReferences;

namespace Stratus
{
  /// <summary>
  /// Allows you to select any registered events within the Editor
  /// </summary>
  [Serializable]
  public class EventField : ISerializationCallbackReceiver
  {
    [ClassExtends(typeof(Stratus.Event), Grouping = ClassGrouping.ByNamespace, AllowAbstract = false)]
    [Tooltip("What type of event this trigger will activate on")]
    public ClassTypeReference type;

    //[SerializeField]
    //public Type type = null;    

    public void OnBeforeSerialize()
    {      
    }

    public void OnAfterDeserialize()
    {
    }
  }

}