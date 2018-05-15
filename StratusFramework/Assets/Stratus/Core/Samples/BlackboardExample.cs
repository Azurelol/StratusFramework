using UnityEngine;
using Stratus;
using Stratus.AI;
using Stratus.Types;

namespace Stratus
{
  namespace Examples
  {
    public class BlackboardExample : StratusBehaviour
    {
      public Blackboard blackboard;
      public string key;
      public Blackboard.Selector selector = new Blackboard.Selector();
      public RuntimeMethodField runtimeMethod;

      private void Awake()
      {
        runtimeMethod = new RuntimeMethodField(GetValue);
      }

      void GetValue()
      {
        object value = blackboard.GetLocal(gameObject, key);
        Trace.Script($"The value of {key} is {value}", this);
      }
    } 
  } 
}
