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
      public string keyName;
      public RuntimeMethodField runtimeMethod;

      private void Awake()
      {
        //blackboard = blackboard.Instantiate();        
        //blackboard = ScriptableObject.Instantiate(blackboard);
        runtimeMethod = new RuntimeMethodField(GetValue);
        //Trace.Script("Boop");
      }

      void GetValue()
      {
        object value = blackboard.GetLocalValue(this, keyName);
        Trace.Script($"The value of {keyName} is {value}", this);
        //int variantValue = blackboard.locals.GetValue<int>(keyName);
        //var variantValue = blackboard.GetLocals(this).Get
      }
    } 
  } 
}
