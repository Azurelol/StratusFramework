using UnityEngine;
using System.Reflection;

namespace Stratus
{
  /// <summary>
  /// Used for interpolating a float value
  /// </summary>
  public class StratusActionPropertyFloat : StratusActionPropertyBase<float>
  {
    public StratusActionPropertyFloat(object target, PropertyInfo property, float endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyFloat(object target, FieldInfo field, float endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }
    public override float ComputeCurrentValue(float easeVal)
    {
      var currentVal = this.initialValue + this.difference * easeVal;
      //Stratus.Trace.Script("currentVal = " + currentVal);
      return currentVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Vector2 value
  /// </summary>
  public class StratusActionPropertyVector2 : StratusActionPropertyBase<Vector2>
  {
    public StratusActionPropertyVector2(object target, PropertyInfo property, Vector2 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyVector2(object target, FieldInfo field, Vector2 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

    public override Vector2 ComputeCurrentValue(float easeVal)
    {
      return this.initialValue + this.difference * easeVal;
    }

  }

  /// <summary>
  /// Used for interpolating a Vector3 value
  /// </summary>
  public class StratusActionPropertyVector3 : StratusActionPropertyBase<Vector3>
  {
    public StratusActionPropertyVector3(object target, PropertyInfo property, Vector3 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyVector3(object target, FieldInfo field, Vector3 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

    public override Vector3 ComputeCurrentValue(float easeVal)
    {
      return this.initialValue + this.difference * easeVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Vector4 value
  /// </summary>
  public class StratusActionPropertyVector4 : StratusActionPropertyBase<Vector4>
  {
    public StratusActionPropertyVector4(object target, PropertyInfo property, Vector4 endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyVector4(object target, FieldInfo field, Vector4 endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

    public override Vector4 ComputeCurrentValue(float easeVal)
    {
      return this.initialValue + this.difference * easeVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Color value
  /// </summary>
  public class StratusActionPropertyColor : StratusActionPropertyBase<Color>
  {
    public StratusActionPropertyColor(object target, PropertyInfo property, Color endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyColor(object target, FieldInfo field, Color endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() {
      this.difference = this.endValue - this.initialValue;      
    }

    public override Color ComputeCurrentValue(float easeVal)
    {
      return this.initialValue + this.difference * easeVal;
    }
  }

  /// <summary>
  /// Used for interpolating a Quaternion value
  /// </summary>
  public class StratusActionPropertyQuaternion : StratusActionPropertyBase<Quaternion>
  {
    public StratusActionPropertyQuaternion(object target, PropertyInfo property, Quaternion endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyQuaternion(object target, FieldInfo field, Quaternion endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { }
    public override Quaternion ComputeCurrentValue(float easeVal) { return new Quaternion(); }
    public override void SetCurrent()
    {
      //Debug.Log("Setting!");
      Set(Quaternion.Lerp(initialValue, endValue, Time.time * Time.deltaTime));
    }

    public override void SetLast()
    {
      Set(endValue);
    }
  }


  /// <summary>
  /// Used for interpolating a boolean value
  /// </summary>
  public class StratusActionPropertyBool : StratusActionPropertyBase<bool>
  {
    public StratusActionPropertyBool(object target, PropertyInfo property, bool endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyBool(object target, FieldInfo field, bool endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { }
    public override void SetCurrent() { }
    public override bool ComputeCurrentValue(float easeVal) { return false; }
  }

  /// <summary>
  /// Used for interpolating an integer value
  /// </summary>
  public class StratusActionPropertyInt : StratusActionPropertyBase<int>
  {
    float CurrentValue;

    public StratusActionPropertyInt(object target, PropertyInfo property, int endValue, float duration, Ease ease)
      : base(target, property, endValue, duration, ease) { }

    public StratusActionPropertyInt(object target, FieldInfo field, int endValue, float duration, Ease ease)
      : base(target, field, endValue, duration, ease) { }

    public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

    public override int ComputeCurrentValue(float easeVal)
    {
      this.CurrentValue = this.initialValue + this.difference * easeVal;
      return Mathf.CeilToInt(this.CurrentValue);
    }

  }
}