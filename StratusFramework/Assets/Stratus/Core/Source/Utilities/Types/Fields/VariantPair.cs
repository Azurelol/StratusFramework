using UnityEngine;
using System;

namespace Stratus
{
  namespace Types
  {
    /// <summary>
    /// A pair between a variant an a generic key
    /// </summary>
    /// <typeparam name="KeyType"></typeparam>
    public class VariantPair<KeyType> where KeyType : IComparable
    {
      //--------------------------------------------------------------------/
      // Fields
      //--------------------------------------------------------------------/
      /// <summary>
      /// The key used for this variant pair
      /// </summary>
      public KeyType key;
      /// <summary>
      /// The variant used by the pair
      /// </summary>
      public Variant value;

      //--------------------------------------------------------------------/
      // Properties
      //--------------------------------------------------------------------/
      /// <summary>
      /// The current type for this variant pair
      /// </summary>
      public Variant.Types type { get { return value.currentType; } }

      //--------------------------------------------------------------------/
      // Constructors
      //--------------------------------------------------------------------/
      public VariantPair(KeyType key, int value) { this.key = key; this.value = new Variant(value); }
      public VariantPair(KeyType key, float value) { this.key = key; this.value = new Variant(value); }
      public VariantPair(KeyType key, bool value) { this.key = key; this.value = new Variant(value); }
      public VariantPair(KeyType key, string value) { this.key = key; this.value = new Variant(value); }
      public VariantPair(KeyType key, Vector3 value) { this.key = key; this.value = new Variant(value); }
      public VariantPair(KeyType key, Variant value) { this.key = key; this.value = new Variant(value); }
      public VariantPair(VariantPair<KeyType> other) { key = other.key; value = new Variant(other.value); }
      
      //--------------------------------------------------------------------/
      // Methods
      //--------------------------------------------------------------------/
      public ValueType GetValue<ValueType>()
      {
        return value.Get<ValueType>();
      }

      public void SetValue<ValueType>(ValueType value)
      {
        this.value.Set(value);
      }

      public bool Compare(VariantPair<KeyType> other) 
      {
        // https://msdn.microsoft.com/en-us/library/system.icomparable(v=vs.110).aspx
        if (this.key.CompareTo(other.key) < 0)
          return false;

        return this.value.Compare(other.value);
      }

      public VariantPair<KeyType> Copy()
      {
        return new VariantPair<KeyType>(key, value);
      }

      public override string ToString()
      {
        return this.key + " = " + value.ToString();
      }
    }  
  }
}
