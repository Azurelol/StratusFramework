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
      public int intValue = 5;

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
        switch (scope)
        {
          case Blackboard.Scope.Local:
            // ... to the table for local symbols in the blackboard, instantiated for
            // each GameObject on access
            blackboard.SetLocal(gameObject, key, intValue);
            break;
          case Blackboard.Scope.Global:
            // ... to the table for global symbols in the blackboard
            blackboard.SetGlobal(key, intValue);
            break;
        }

      }

      private void SetValueWithSelector()
      {
        // Example of how such a value would be set using a selector
        selector.Set(gameObject, intValue);
      }

      //--------------------------------------------------------------------/
      // Callbacks
      //--------------------------------------------------------------------/
      private void OnLocalSymbolChanged(GameObject gameObject, Symbol symbol)
      {
        Trace.Script($"The value on local symbol {symbol.key} on the GameObject {gameObject} was changed to {symbol.value}", this);
      }

      private void OnGlobalSymbolChanged(Symbol symbol)
      {
        Trace.Script($"The value on global symbol {symbol} was changed to {symbol.value}", this);
      }

    }
  }
}
