using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// A pair between a variant an a generic key
	/// </summary>
	/// <typeparam name="KeyType"></typeparam>
	public class StratusKeyVariantPair<KeyType> where KeyType : IComparable
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
		public StratusVariant value;

		//--------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------/
		/// <summary>
		/// The current type for this variant pair
		/// </summary>
		public StratusVariant.VariantType type { get { return value.currentType; } }

		/// <summary>
		/// Information about the symbol
		/// </summary>
		public string annotation => $"{key} ({value.currentType})";

		//--------------------------------------------------------------------/
		// Constructors
		//--------------------------------------------------------------------/
		public StratusKeyVariantPair(KeyType key, int value) { this.key = key; this.value = new StratusVariant(value); }
		public StratusKeyVariantPair(KeyType key, float value) { this.key = key; this.value = new StratusVariant(value); }
		public StratusKeyVariantPair(KeyType key, bool value) { this.key = key; this.value = new StratusVariant(value); }
		public StratusKeyVariantPair(KeyType key, string value) { this.key = key; this.value = new StratusVariant(value); }
		public StratusKeyVariantPair(KeyType key, Vector3 value) { this.key = key; this.value = new StratusVariant(value); }
		public StratusKeyVariantPair(KeyType key, StratusVariant value) { this.key = key; this.value = new StratusVariant(value); }
		public StratusKeyVariantPair(StratusKeyVariantPair<KeyType> other) { key = other.key; value = new StratusVariant(other.value); }

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

		public bool Compare(StratusKeyVariantPair<KeyType> other)
		{
			// https://msdn.microsoft.com/en-us/library/system.icomparable(v=vs.110).aspx
			if (this.key.CompareTo(other.key) < 0)
				return false;

			return this.value.Compare(other.value);
		}

		public StratusKeyVariantPair<KeyType> Copy()
		{
			return new StratusKeyVariantPair<KeyType>(key, value);
		}

		public override string ToString()
		{
			return $"{this.key} ({value.ToString()})";
		}
	}
}
