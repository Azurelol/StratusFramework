/******************************************************************************/
/*!
@file   ReferenceType.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  public class ReferenceType<T>
  {
    public T Value;
    //------------------------------------------------------------------------/
    public delegate void Setter(T val);
    public void Set(T val) { this.Value = val; }
    public delegate T Getter();
    public T Get() { return this.Value; }
    //------------------------------------------------------------------------/
    public ReferenceType(T val) { Value = val; }
    public override string ToString()
    {
      return Value.ToString();
    }
  }

  public class Integer : ReferenceType<int>
  {
    public Integer(int val) : base(val) {}
    public static implicit operator Integer(int val) { return new Integer(val); }
    public static implicit operator int(Integer refType) { return refType.Value; }
  }

  public class Real :  ReferenceType<float>
  {
    public Real(float val) : base(val) { }
    public static implicit operator Real(float val) { return new Real(val); }
    public static implicit operator float(Real refType) { return refType.Value; }
  }

  public class Boolean : ReferenceType<bool>
  {
    public Boolean(bool val) : base(val) { }
    public static implicit operator Boolean(bool val) { return new Boolean(val); }
    public static implicit operator bool(Boolean refType) { return refType.Value; }
  }

  public class Real2 : ReferenceType<Vector2>
  { 
    public Real2() : base(new Vector2()) {}
    public Real2(Vector2 val) : base(val) { }
    public static implicit operator Real2(Vector2 val) { return new Real2(val); }
    public static implicit operator Vector2(Real2 refType) { return refType.Value; }
  }

  public class Real3 : ReferenceType<Vector3>
  {
    public Real3() : base(new Vector3()) { }
    public Real3(Vector3 val) : base(val) { }
    public static implicit operator Real3(Vector3 val) { return new Real3(val); }
    public static implicit operator Vector3(Real3 refType) { return refType.Value; }
  }

  public class Real4 : ReferenceType<Vector4>
  {
    public Real4() : base(new Vector4()) { }
    public Real4(Vector4 val) : base(val) { }
    public static implicit operator Real4(Vector4 val) { return new Real4(val); }
    public static implicit operator Vector4(Real4 refType) { return refType.Value; }
  }

}