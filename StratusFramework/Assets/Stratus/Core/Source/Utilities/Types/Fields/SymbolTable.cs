#define STRATUS_SYMBOLTABLE_USEMAP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;


namespace Stratus.Types
{
  /// <summary>
  /// An internally-managed list of symbols
  /// </summary>
  [Serializable]
  public class SymbolTable : IEnumerable<Symbol>
  {
    //--------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------/
    public delegate void OnSymbolChanged(Symbol symbol);

    //--------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------/
    [SerializeField]
    protected List<Symbol> symbols = new List<Symbol>();

    //--------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------/
    /// <summary>
    /// The lookup table for quick access to symbols
    /// </summary>
    private Dictionary<string, Symbol> symbolsMap { get; set; }

    /// <summary>
    /// Whether the lookupp table has been initialized. It will be initialized
    /// on the very first find attempt.
    /// </summary>
    private bool hasLookupTable { get; set; }

    /// <summary>
    /// True if the table is empty of symbols
    /// </summary>
    public bool isEmpty { get { return symbols.Count == 0; } }

    /// <summary>
    /// The names of all keys for all symbols in this table
    /// </summary>
    public string[] keys
    {
      get
      {
        string[] values = new string[symbols.Count];
        for (int i = 0; i < symbols.Count; ++i)
        {
          values[i] = symbols[i].key;
        }
        return values;
      }
    }

    /// <summary>
    /// References to all the current symbols in this table
    /// </summary>
    public Symbol.Reference[] references
    {
      get
      {
        Symbol.Reference[] values = new Symbol.Reference[symbols.Count];
        for (int i = 0; i < symbols.Count; ++i)
        {
          values[i] = symbols[i].reference;
        }
        return values;
      }
    }

    /// <summary>
    /// The names of all keys for all symbols in this table
    /// </summary>
    public string[] keysAnnotated
    {
      get
      {
        string[] values = new string[symbols.Count];
        for (int i = 0; i < symbols.Count; ++i)
        {
          values[i] = symbols[i].annotation;
        }
        return values;
      }
    }

    //--------------------------------------------------------------------/
    // Constructors
    //--------------------------------------------------------------------/        
    public SymbolTable()
    {
    }

    public SymbolTable(SymbolTable other)
    {
      symbols = other.symbols.ConvertAll(symbol => new Symbol(symbol));
    }

    //--------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------/
    /// <summary>
    /// Retrieves the value of a given symbol in the table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetValue<T>(string key)
    {
      Symbol symbol = Find(key);
      return symbol.value.Get<T>();
    }

    /// <summary>
    /// Retrieves the value of a given symbol in the table
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetValue(string key)
    {
      // Look for the key in the list
      Symbol symbol = Find(key);
      return symbol.value.Get();
    }

    /// <summary>
    /// Sets the value of a given symbol in the table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetValue<T>(string key, T value)
    {
      // Look for the key in the list
      Symbol symbol = Find(key);
      symbol.SetValue<T>(value);
    }

    /// <summary>
    /// Sets the value of a given symbol in the table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetValue(string key, object value)
    {
      // Look for the key in the list
      Symbol symbol = Find(key);
      symbol.SetValue(value);
    }

    /// <summary>
    /// Retrieves the symbol using the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Symbol Find(string key)
    {
      #if STRATUS_SYMBOLTABLE_USEMAP

      // Construct the lookup table if not set yet
      if (!hasLookupTable)
        ConstructLookupTable();

      // Look for the key in the list
      if (!symbolsMap.ContainsKey(key))
        throw new KeyNotFoundException("The key '" + key + "' was not found on this symbol table!");
      return symbolsMap[key];

      #else
      var symbol = symbols.Find(x => x.key == key);
      if (symbol != null)
        return symbol;
      return null;
      #endif
    }

    /// <summary>
    /// Checks whether this table contains the symbol with the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(string key)
    {
      Symbol symbol = Find(key);
      return symbol != null;
    }

    /// <summary>
    /// Adds a symbol to this table at runtime
    /// </summary>
    /// <param name="symbol"></param>
    public void Add(Symbol symbol)
    {
      symbols.Add(symbol);
      symbolsMap.Add(symbol.key, symbol);
    }

    /// <summary>
    /// Validates this symbol table, ensuring there's no duplicate keys
    /// </summary>
    /// <returns></returns>
    public bool Assert()
    {
      return symbols.HasDuplicateKeys((Symbol s) => s.key);
    }

    /// <summary>
    /// Constructs the lookup table used by this symbol table for quick access
    /// </summary>
    private void ConstructLookupTable()
    {
      symbolsMap = new Dictionary<string, Symbol>();
      foreach (var symbol in symbols)
        symbolsMap.Add(symbol.key, symbol);
      hasLookupTable = true;
    }

    //--------------------------------------------------------------------/
    // Interface
    //--------------------------------------------------------------------/
    /// <summary>
    /// Prints all the symbols along with their values
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      var builder = new StringBuilder();
      foreach (var symbol in symbols)
      {
        builder.AppendFormat(" - {0}", symbol.ToString());
      }
      return builder.ToString();
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
      return ((IEnumerable<Symbol>)this.symbols).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<Symbol>)this.symbols).GetEnumerator();
    }
  }
}