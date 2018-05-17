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
      [Header("Default")]
      public Blackboard blackboard;
      public Blackboard.Scope scope;
      public string key = "Dogs";

      [Header("Selector")]
      public Blackboard.Selector selector = new Blackboard.Selector();
      public RuntimeMethodField runtimeMethod;
            

      private void Awake()
      {
        runtimeMethod = new RuntimeMethodField(GetValue, GetValueWithSelector, SetValue);
        blackboard.onLocalSymbolChanged += OnLocalSymbolChanged;
        blackboard.onGlobalSymbolChanged += OnGlobalSymbolChanged;
      }
      
      // Examples -----------------------------------------------------------
      //--------------------------------------------------------------------/
      // Retrieving Values
      //--------------------------------------------------------------------/
      private void GetValue()
      {
        object value = null;
        switch (scope)
        {
          case Blackboard.Scope.Local:
            value = blackboard.GetLocal(gameObject, key);
            break;
          case Blackboard.Scope.Global:
            value = blackboard.GetGlobal(key);
            break;
          default:
            break;
        }
        Trace.Script($"The value of {key} is {value}", this);
      }

      private void GetValueWithSelector()
      {
        object value = selector.Get(gameObject);
        Trace.Script($"The value of {selector.key} is {value}", this);
      }

      //--------------------------------------------------------------------/
      // Setting Values
      //--------------------------------------------------------------------/
      private void SetValue()
      {
        // Example of how such a value would be set...
        int intValue = 5;
        // ... to the table for local symbols in the blackboard, instantiated for
        // each GameObject on access
        blackboard.SetLocal(gameObject, key, intValue);
        // ... to the table for global symbols in the blackboard
//        blackboard.SetGlobal(key, intValue);
      }

      private void SetValueWithSelector()
      {
        // Example of how such a value would be set using a selector
        int intValue = 5;
        selector.Set(gameObject, intValue);
      }

      //--------------------------------------------------------------------/
      // Callbacks
      //--------------------------------------------------------------------/
      private void OnLocalSymbolChanged(GameObject gameObject, Symbol symbol)
      {
        Trace.Script($"The value on local symbol {symbol.key} on the GameObject {gameObject} was changed", this);
      }

      private void OnGlobalSymbolChanged(Symbol symbol)
      {
        Trace.Script($"The value on global symbol {symbol} was changed!", this);
      }

    }
  } 
}
