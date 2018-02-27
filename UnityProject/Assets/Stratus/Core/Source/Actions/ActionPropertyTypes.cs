/******************************************************************************/
/*!
@file   ActionPropertyFloat.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Reflection;

namespace Stratus
{
  /// <summary>
  /// Used for interpolating a float value
  /// </summary>
  public class ActionPropertyFloat : ActionPropertyBase<float>
  {
    public ActionPropertyFloat(object target, PropertyInfo property, float endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyFloat(object target, FieldInfo field, float endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }
    public override float ComputeCurrentValue(float easeVal)
    {
      var currentVal = this.InitialValue + this.Difference * easeVal;
      //Stratus.Trace.Script("currentVal = " + currentVal);
      return currentVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Vector2 value
  /// </summary>
  public class ActionPropertyVector2 : ActionPropertyBase<Vector2>
  {
    public ActionPropertyVector2(object target, PropertyInfo property, Vector2 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyVector2(object target, FieldInfo field, Vector2 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override Vector2 ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }

  }

  /// <summary>
  /// Used for interpolating a Vector3 value
  /// </summary>
  public class ActionPropertyVector3 : ActionPropertyBase<Vector3>
  {
    public ActionPropertyVector3(object target, PropertyInfo property, Vector3 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyVector3(object target, FieldInfo field, Vector3 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override Vector3 ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Vector4 value
  /// </summary>
  public class ActionPropertyVector4 : ActionPropertyBase<Vector4>
  {
    public ActionPropertyVector4(object target, PropertyInfo property, Vector4 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyVector4(object target, FieldInfo field, Vector4 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override Vector4 ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Color value
  /// </summary>
  public class ActionPropertyColor : ActionPropertyBase<Color>
  {
    public ActionPropertyColor(object target, PropertyInfo property, Color endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyColor(object target, FieldInfo field, Color endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() {
      this.Difference = this.EndValue - this.InitialValue;      
    }

    public override Color ComputeCurrentValue(float easeVal)
    {
      return this.InitialValue + this.Difference * easeVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Quaternion value
  /// </summary>
  public class ActionPropertyQuaternion : ActionPropertyBase<Quaternion>
  {
    public ActionPropertyQuaternion(object target, PropertyInfo property, Quaternion endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyQuaternion(object target, FieldInfo field, Quaternion endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { }
    public override Quaternion ComputeCurrentValue(float easeVal) { return new Quaternion(); }
    public override void SetCurrent()
    {
      //Debug.Log("Setting!");
      Set(Quaternion.Lerp(InitialValue, EndValue, Time.time * Time.deltaTime));
    }

    public override void SetLast()
    {
      Set(EndValue);
    }
  }


  /// <summary>
  /// Used for interpolating a boolean value
  /// </summary>
  public class ActionPropertyBool : ActionPropertyBase<bool>
  {
    public ActionPropertyBool(object target, PropertyInfo property, bool endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyBool(object target, FieldInfo field, bool endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { }
    public override void SetCurrent() { }
    public override bool ComputeCurrentValue(float easeVal) { return false; }
  }

  /// <summary>
  /// Used for interpolating an integer value
  /// </summary>
  public class ActionPropertyInt : ActionPropertyBase<int>
  {
    float CurrentValue;

    public ActionPropertyInt(object target, PropertyInfo property, int endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public ActionPropertyInt(object target, FieldInfo field, int endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.Difference = this.EndValue - this.InitialValue; }

    public override int ComputeCurrentValue(float easeVal)
    {
      this.CurrentValue = this.InitialValue + this.Difference * easeVal;
      return Mathf.CeilToInt(this.CurrentValue);
    }

  }
}