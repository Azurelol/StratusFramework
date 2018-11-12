using UnityEngine;

namespace Stratus
{
	public class ReferenceType<T>
	{
		//----------------------------------------------------------------------/
		// Fields
		//----------------------------------------------------------------------/
		public T value { get; set; }

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/
		public delegate void Setter(T val);
		public delegate T Getter();

		public void Set(T val) { this.value = val; }
		public T Get() { return this.value; }

		//----------------------------------------------------------------------/
		// CTOR
		//----------------------------------------------------------------------/
		public ReferenceType(T val) { this.value = val; }

		//----------------------------------------------------------------------/
		// Messages
		//----------------------------------------------------------------------/
		public override string ToString()
		{
			return this.value.ToString();
		}
	}
	public class IntegerRef : ReferenceType<int>
	{
		public IntegerRef(int val) : base(val) { }
		public static implicit operator IntegerRef(int val) { return new IntegerRef(val); }
		public static implicit operator int(IntegerRef refType) { return refType.value; }
	}
	public class FloatRef : ReferenceType<float>
	{
		public FloatRef(float val) : base(val) { }
		public static implicit operator FloatRef(float val) { return new FloatRef(val); }
		public static implicit operator float(FloatRef refType) { return refType.value; }
	}

	public class BooleanRef : ReferenceType<bool>
	{
		public BooleanRef(bool val) : base(val) { }
		public static implicit operator BooleanRef(bool val) { return new BooleanRef(val); }
		public static implicit operator bool(BooleanRef refType) { return refType.value; }
	}

	public class Vector2Ref : ReferenceType<Vector2>
	{
		public Vector2Ref() : base(new Vector2()) { }
		public Vector2Ref(Vector2 val) : base(val) { }
		public static implicit operator Vector2Ref(Vector2 val) { return new Vector2Ref(val); }
		public static implicit operator Vector2(Vector2Ref refType) { return refType.value; }
	}

	public class Vector3Ref : ReferenceType<Vector3>
	{
		public Vector3Ref() : base(new Vector3()) { }
		public Vector3Ref(Vector3 val) : base(val) { }
		public static implicit operator Vector3Ref(Vector3 val) { return new Vector3Ref(val); }
		public static implicit operator Vector3(Vector3Ref refType) { return refType.value; }
	}

	public class Vector4Ref : ReferenceType<Vector4>
	{
		public Vector4Ref() : base(new Vector4()) { }
		public Vector4Ref(Vector4 val) : base(val) { }
		public static implicit operator Vector4Ref(Vector4 val) { return new Vector4Ref(val); }
		public static implicit operator Vector4(Vector4Ref refType) { return refType.value; }
	}

}