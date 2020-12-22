using UnityEngine;
using Stratus.Types;

namespace Stratus
{
  namespace Examples
  {
    public class BlackboardExample : StratusBehaviour
    {
      [Header("Default")]
      public StratusBlackboard blackboard;
      public StratusBlackboard.Scope scope;
      public string key = "Dogs";
      public int intValue = 5;

      [Header("Selector")]
      public StratusBlackboard.Selector selector = new StratusBlackboard.Selector();
      public StratusRuntimeMethodField runtimeMethod;


      private void Awake()
      {
        runtimeMethod = new StratusRuntimeMethodField(GetValue, GetValueWithSelector, SetValue);
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
          case StratusBlackboard.Scope.Local:
            value = blackboard.GetLocal(gameObject, key);
            break;
          case StratusBlackboard.Scope.Global:
            value = blackboard.GetGlobal(key);
            break;
        }
        StratusDebug.Log($"The value of {key} is {value}", this);
      }

      private void GetValueWithSelector()
      {
        object value = selector.Get(gameObject);
        StratusDebug.Log($"The value of {selector.key} is {value}", this);
      }

      //--------------------------------------------------------------------/
      // Setting Values
      //--------------------------------------------------------------------/
      private void SetValue()
      {
        // Example of how such a value would be set...
        switch (scope)
        {
          case StratusBlackboard.Scope.Local:
            // ... to the table for local symbols in the blackboard, instantiated for
            // each GameObject on access
            blackboard.SetLocal(gameObject, key, intValue);
            break;
          case StratusBlackboard.Scope.Global:
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
      private void OnLocalSymbolChanged(GameObject gameObject, StratusSymbol symbol)
      {
        StratusDebug.Log($"The value on local symbol {symbol.key} on the GameObject {gameObject} was changed to {symbol.value}", this);
      }

      private void OnGlobalSymbolChanged(StratusSymbol symbol)
      {
        StratusDebug.Log($"The value on global symbol {symbol} was changed to {symbol.value}", this);
      }

    }
  }
}
