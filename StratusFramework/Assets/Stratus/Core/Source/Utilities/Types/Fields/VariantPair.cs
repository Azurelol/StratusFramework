using UnityEngine;
using System;

namespace Stratus
{
  namespace Types
  {
    public class VariantPair<KeyType> where KeyType : IComparable
    {
      public KeyType Key;
      public Variant Value;
      public Variant.Types Type { get { return Value.CurrentType; } }

      public VariantPair(KeyType key, int value) { Key = key; Value = new Variant(value); }
      public VariantPair(KeyType key, float value) { Key = key; Value = new Variant(value); }
      public VariantPair(KeyType key, bool value) { Key = key; Value = new Variant(value); }
      public VariantPair(KeyType key, string value) { Key = key; Value = new Variant(value); }
      public VariantPair(KeyType key, Vector3 value) { Key = key; Value = new Variant(value); }
      public VariantPair(KeyType key, Variant value) { Key = key; Value = new Variant(value); }
      public VariantPair(VariantPair<KeyType> other) { Key = other.Key; Value = new Variant(other.Value); }

      public ValueType GetValue<ValueType>()
      {
        return Value.Get<ValueType>();
      }

      public void SetValue<ValueType>(ValueType value)
      {
        Value.Set<ValueType>(value);
      }

      public bool Compare(VariantPair<KeyType> other) 
      {
        // https://msdn.microsoft.com/en-us/library/system.icomparable(v=vs.110).aspx
        if (this.Key.CompareTo(other.Key) < 0)
          return false;

        return this.Value.Compare(other.Value);
      }

      public VariantPair<KeyType> Copy()
      {
        return new VariantPair<KeyType>(Key, Value);
      }

      public override string ToString()
      {
        return this.Key + " = " + Value.ToString();
      }
    }  
  }
}
