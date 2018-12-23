using UnityEngine;
using Stratus;
using System;
using System.Runtime.InteropServices;

class UnionTest : MonoBehaviour
{

  //[StructLayout(LayoutKind.Explicit)]
  //public struct Union
  //{
  //  public enum Types
  //  {
  //    Integer, Boolean, Float, String, Vector3
  //  }

  //  [FieldOffset(0)]
  //  [MarshalAs(UnmanagedType.I4)]
  //  private Types Type;

  //  [FieldOffset(4)]
  //  private int Integer;

  //  [FieldOffset(4)]
  //  private float Float;

  //  [FieldOffset(4)]
  //  private bool Boolean;

  //  [FieldOffset(4)]
  //  private string String;

  //  [FieldOffset(4)]
  //  private Vector3 Vector3;
  //}


  //public Union Test;

  [StructLayout(LayoutKind.Explicit)]
  public struct MyUnion
  {
    public enum Types
    {
      Integer, Boolean, Float, String, Vector3
    }
    
    [SerializeField]    
    [FieldOffset(0)]
    private Types T;

    [SerializeField]
    [FieldOffset(4)]
    private int I;

    [SerializeField]
    [FieldOffset(4)]
    private float F;

    [SerializeField]
    [FieldOffset(4)]
    private bool B;

    [SerializeField]
    [FieldOffset(4)]
    private Vector3 V;

    [SerializeField]
    [FieldOffset(16)]
    //[MarshalAs(UnmanagedType.AnsiBStr)]
    private string S;
  }

  MyUnion Test;

  private void Start()
  {
    //string s = "hi";
    //var size = Marshal.SizeOf(s);
    //Trace.Script("size = " + size);
  }


}
