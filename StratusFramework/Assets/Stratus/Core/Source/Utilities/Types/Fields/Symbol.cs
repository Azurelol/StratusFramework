using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Stratus
{
  namespace Types
  {
    /// <summary>
    /// A symbol represents a key-value pair where the key is an identifying string
    /// and the value is a variant (which can represent multiple types)
    /// </summary>
    [Serializable]
    public class Symbol : VariantPair<string>
    {
      //--------------------------------------------------------------------/
      // Constructors
      //--------------------------------------------------------------------/
      public Symbol(string key, int value) : base(key, value) { }
      public Symbol(string key, float value) : base(key, value) { }
      public Symbol(string key, bool value) : base(key, value) { }
      public Symbol(string key, string value) : base(key, value) { }
      public Symbol(string key, Vector3 value) : base(key, value) { }
      public Symbol(string key, Variant value) : base(key, value) { }
      public Symbol(Symbol other) : base(other) { }

      //--------------------------------------------------------------------/
      // Methods
      //--------------------------------------------------------------------/
      /// <summary>
      /// Constructs a symbol with the given key and value
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="key"></param>
      /// <param name="value"></param>
      /// <returns></returns>
      public static Symbol Construct<T>(string key, T value)
      {
        return new Symbol(key, Variant.Make(value));
      }

      //--------------------------------------------------------------------/
      // Properties
      //--------------------------------------------------------------------/
      /// <summary>
      /// Constructs a reference to this symbol
      /// </summary>
      /// <returns></returns>
      public Reference reference => new Reference() { key = key, type = type }; 

      /// <summary>
      /// A reference of a symbol
      /// </summary>
      [Serializable]
      public class Reference
      {
        public string key;        
        public Variant.Types type;
      }

      /// <summary>
      /// An internally-managed list of symbols
      /// </summary>
      [Serializable]
      public class Table : IEnumerable<Symbol>
      {
        //--------------------------------------------------------------------/
        // Fields
        //--------------------------------------------------------------------/
        [SerializeField]
        protected List<Symbol> symbols = new List<Symbol>();

        //--------------------------------------------------------------------/
        // Properties
        //--------------------------------------------------------------------/
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
            for(int i = 0; i < symbols.Count; ++i)
            {
              values[i] = symbols[i].key;
            }
            return values;
          }
        }

        /// <summary>
        /// References to all the current symbols in this table
        /// </summary>
        public Reference[] references
        {
          get
          {
            Reference[] values = new Reference[symbols.Count];
            for (int i = 0; i < symbols.Count; ++i)
            {
              values[i] = symbols[i].reference;
            }
            return values;
          }
        }

        //--------------------------------------------------------------------/
        // Constructors
        //--------------------------------------------------------------------/        
        public Table()
        {
        }

        public Table(Table other)
        {
          symbols = other.symbols.ConvertAll(symbol => new Symbol(symbol));
        }

        //--------------------------------------------------------------------/
        // Methods
        //--------------------------------------------------------------------/
        public T GetValue<T>(string key)
        {
          // Look for the key in the list
          var symbol = symbols.Find(x => x.key == key);
          if (symbol != null)
            return symbol.value.Get<T>();

          throw new KeyNotFoundException("The key '" + key + "' was not found!");
        }

        public object GetValue(string key)
        {
          // Look for the key in the list
          var symbol = symbols.Find(x => x.key == key);
          if (symbol != null)
            return symbol.value.Get();

          throw new KeyNotFoundException("The key '" + key + "' was not found!");
        }
        

        public void SetValue<T>(string key, T value)
        {
          // Look for the key in the list
          var variantPair = symbols.Find(x => x.key == key);
          if (variantPair != null)
            variantPair.SetValue<T>(value);

          throw new KeyNotFoundException("The key '" + key + "' was not found!");
        }

        public Symbol Find(string key)
        {
          var symbol = symbols.Find(x => x.key == key);
          if (symbol != null)
            return symbol;
          return null;
        }

        public bool Contains(string key)
        {
          var symbol = symbols.Find(x => x.key == key);
          if (symbol != null)
            return true;
          return false;
        }

        public void Add(Symbol symbol)
        {
          symbols.Add(symbol);
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

  }
}
