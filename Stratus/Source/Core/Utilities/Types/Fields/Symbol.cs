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
      public Symbol(string key, int value) : base(key, value) { }
      public Symbol(string key, float value) : base(key, value) { }
      public Symbol(string key, bool value) : base(key, value) { }
      public Symbol(string key, string value) : base(key, value) { }
      public Symbol(string key, Vector3 value) : base(key, value) { }
      public Symbol(string key, Variant value) : base(key, value) { }
      public Symbol(Symbol other) : base(other) { }
      public static Symbol Make<T>(string key, T value)
      {
        return new Symbol(key, Variant.Make(value));
      }

      /// <summary>
      /// A reference of a symbol
      /// </summary>
      [Serializable]
      public class Reference
      {
        public string Key;        
        public Variant.Types Type;
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
        protected List<Symbol> Symbols = new List<Symbol>();

        //--------------------------------------------------------------------/
        // Properties
        //--------------------------------------------------------------------/
        /// <summary>
        /// True if the table is empty of symbols
        /// </summary>
        public bool IsEmpty { get { return Symbols.Count == 0; } }

        //--------------------------------------------------------------------/
        // Constructors
        //--------------------------------------------------------------------/        
        public Table()
        {
        }

        public Table(Table other)
        {
          Symbols = other.Symbols.ConvertAll(symbol => new Symbol(symbol));
        }

        //--------------------------------------------------------------------/
        // Methods
        //--------------------------------------------------------------------/
        public T GetValue<T>(string key)
        {
          // Look for the key in the list
          var symbol = Symbols.Find(x => x.Key == key);
          if (symbol != null)
            return symbol.Value.Get<T>();

          throw new KeyNotFoundException("The key '" + key + "' was not found!");
        }

        public void SetValue<T>(string key, T value)
        {
          // Look for the key in the list
          var variantPair = Symbols.Find(x => x.Key == key);
          if (variantPair != null)
            variantPair.SetValue<T>(value);

          throw new KeyNotFoundException("The key '" + key + "' was not found!");
        }

        public Symbol Find(string key)
        {
          var symbol = Symbols.Find(x => x.Key == key);
          if (symbol != null)
            return symbol;
          return null;
        }

        public bool Contains(string key)
        {
          var symbol = Symbols.Find(x => x.Key == key);
          if (symbol != null)
            return true;
          return false;
        }

        public void Add(Symbol symbol)
        {
          Symbols.Add(symbol);
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
          foreach (var symbol in Symbols)
          {
            builder.AppendFormat(" - {0}", symbol.ToString());
          }
          return builder.ToString();
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
          return ((IEnumerable<Symbol>)this.Symbols).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
          return ((IEnumerable<Symbol>)this.Symbols).GetEnumerator();
        }
      }
    }

  }
}
