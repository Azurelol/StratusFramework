using UnityEngine;

namespace Stratus
{
	public class StratusStructReference<T>
		where T : struct
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
		public StratusStructReference(T val) { this.value = val; }

		//----------------------------------------------------------------------/
		// Messages
		//----------------------------------------------------------------------/
		public override string ToString()
		{
			return this.value.ToString();
		}
	}
	public class StratusIntegerRef : StratusStructReference<int>
	{
		public StratusIntegerRef(int val) : base(val) { }
		public static implicit operator StratusIntegerRef(int val) { return new StratusIntegerRef(val); }
		public static implicit operator int(StratusIntegerRef refType) { return refType.value; }
	}
	public class StratusFloatRef : StratusStructReference<float>
	{
		public StratusFloatRef(float val) : base(val) { }
		public static implicit operator StratusFloatRef(float val) { return new StratusFloatRef(val); }
		public static implicit operator float(StratusFloatRef refType) { return refType.value; }
	}

	public class StratusBooleanRef : StratusStructReference<bool>
	{
		public StratusBooleanRef(bool val) : base(val) { }
		public static implicit operator StratusBooleanRef(bool val) { return new StratusBooleanRef(val); }
		public static implicit operator bool(StratusBooleanRef refType) { return refType.value; }
	}

	public class StratusVector2Ref : StratusStructReference<Vector2>
	{
		public StratusVector2Ref() : base(new Vector2()) { }
		public StratusVector2Ref(Vector2 val) : base(val) { }
		public static implicit operator StratusVector2Ref(Vector2 val) { return new StratusVector2Ref(val); }
		public static implicit operator Vector2(StratusVector2Ref refType) { return refType.value; }
	}

	public class StratusVector3Ref : StratusStructReference<Vector3>
	{
		public StratusVector3Ref() : base(new Vector3()) { }
		public StratusVector3Ref(Vector3 val) : base(val) { }
		public static implicit operator StratusVector3Ref(Vector3 val) { return new StratusVector3Ref(val); }
		public static implicit operator Vector3(StratusVector3Ref refType) { return refType.value; }
	}

	public class StratusVector4Ref : StratusStructReference<Vector4>
	{
		public StratusVector4Ref() : base(new Vector4()) { }
		public StratusVector4Ref(Vector4 val) : base(val) { }
		public static implicit operator StratusVector4Ref(Vector4 val) { return new StratusVector4Ref(val); }
		public static implicit operator Vector4(StratusVector4Ref refType) { return refType.value; }
	}

}