using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Stratus
{
	/// <summary>
	/// A symbol represents a key-value pair where the key is an identifying string
	/// and the value is a variant (which can represent multiple types)
	/// </summary>
	[Serializable]
	public class StratusSymbol : StratusKeyVariantPair<string>
	{
		//--------------------------------------------------------------------/
		// Constructors
		//--------------------------------------------------------------------/
		public StratusSymbol(string key, int value) : base(key, value) { }
		public StratusSymbol(string key, float value) : base(key, value) { }
		public StratusSymbol(string key, bool value) : base(key, value) { }
		public StratusSymbol(string key, string value) : base(key, value) { }
		public StratusSymbol(string key, Vector3 value) : base(key, value) { }
		public StratusSymbol(string key, StratusVariant value) : base(key, value) { }
		public StratusSymbol(StratusSymbol other) : base(other) { }

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
		public static StratusSymbol Construct<T>(string key, T value)
		{
			return new StratusSymbol(key, StratusVariant.Make(value));
		}

		//--------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------/
		/// <summary>
		/// Constructs a reference to this symbol
		/// </summary>
		/// <returns></returns>
		public Reference reference => new Reference(key, type);

		/// <summary>
		/// A reference of a symbol
		/// </summary>
		[Serializable]
		public class Reference
		{
			public string key;
			public StratusVariant.VariantType type;

			public Reference()
			{
			}

			public Reference(StratusVariant.VariantType type)
			{
				this.type = type;
			}

			public Reference(string key, StratusVariant.VariantType type)
			{
				this.key = key;
				this.type = type;
			}
		}



		/// <summary>
		/// A reference of a symbol
		/// </summary>
		[Serializable]
		public class Vector3Reference
		{
			public string key;
			public StratusVariant.VariantType type { get; } = StratusVariant.VariantType.Vector3;
		}


	}


}
